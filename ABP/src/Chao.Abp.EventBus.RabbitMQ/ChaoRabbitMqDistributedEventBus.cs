using Chao.Abp.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.RabbitMQ;
using Volo.Abp.Security.Claims;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace Chao.Abp.EventBus.RabbitMQ;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IDistributedEventBus), typeof(RabbitMqDistributedEventBus), typeof(IRabbitMqDistributedEventBus))]
public class ChaoRabbitMqDistributedEventBus(IOptions<AbpRabbitMqEventBusOptions> options, IConnectionPool connectionPool, IRabbitMqSerializer serializer, IServiceScopeFactory serviceScopeFactory, IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions, IRabbitMqMessageConsumerFactory messageConsumerFactory, ICurrentTenant currentTenant, IUnitOfWorkManager unitOfWorkManager, IGuidGenerator guidGenerator, IClock clock, IEventHandlerInvoker eventHandlerInvoker, ILocalEventBus localEventBus, ICorrelationIdProvider correlationIdProvider, ICurrentPrincipalAccessor currentPrincipalAccessor, IOptions<ChaoAbpEventBusOption> chaoAbpEventBusOptions, ChaoEventEtoBuilder chaoEventEtoBuilder, DefaultClaimBuilder defaultClaimBuilder) : RabbitMqDistributedEventBus(options, connectionPool, serializer, serviceScopeFactory, distributedEventBusOptions, messageConsumerFactory, currentTenant, unitOfWorkManager, guidGenerator, clock, eventHandlerInvoker, localEventBus, correlationIdProvider)
{
    public override void Initialize()
    {
        var consumer = MessageConsumerFactory.Create(
            new ExchangeDeclareConfiguration(
                AbpRabbitMqEventBusOptions.ExchangeName,
                type: AbpRabbitMqEventBusOptions.GetExchangeTypeOrDefault(),
                durable: true,
                arguments: AbpRabbitMqEventBusOptions.ExchangeArguments
            ),
            new QueueDeclareConfiguration(
                AbpRabbitMqEventBusOptions.ClientName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                prefetchCount: AbpRabbitMqEventBusOptions.PrefetchCount,
                arguments: AbpRabbitMqEventBusOptions.QueueArguments
            ),
            AbpRabbitMqEventBusOptions.ConnectionName
        );

        var consumerField = typeof(RabbitMqDistributedEventBus).GetProperty("Consumer", BindingFlags.Instance | BindingFlags.NonPublic);
        if (consumerField != null)
        {
            consumerField.SetValue(this, consumer);
        }
        Consumer.OnMessageReceived(ProcessEventAsync);

        SubscribeHandlers(AbpDistributedEventBusOptions.Handlers);
    }

    public override async Task ProcessFromInboxAsync(
        IncomingEventInfo incomingEvent,
        InboxConfig inboxConfig)
    {
        var eventType = EventTypes.GetOrDefault(incomingEvent.EventName);
        if (eventType == null)
        {
            return;
        }

        var newEventType = typeof(ChaoEventEto<>).MakeGenericType(eventType);
        var eventData = Serializer.Deserialize(incomingEvent.EventData, newEventType);

        if (eventData is IChaoEventEto chaoEventEto && chaoEventEto.Chao == true)
        {
            using (CurrentTenant.Change(chaoEventEto.TenantId, chaoEventEto.TenantName))
            {
                var claims = defaultClaimBuilder.Build(chaoEventEto.Claims);
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                using (currentPrincipalAccessor.Change(claimsPrincipal))
                {
                    var exceptions = new List<Exception>();
                    using (CorrelationIdProvider.Change(incomingEvent.GetCorrelationId()))
                    {
                        await TriggerHandlersFromInboxAsync(eventType, chaoEventEto.GetEventData(), exceptions, inboxConfig);
                    }
                    if (exceptions.Any())
                    {
                        ThrowOriginalExceptions(eventType, exceptions);
                    }
                }
            }
        }
        else
        {
            eventData = Serializer.Deserialize(incomingEvent.EventData, eventType);
            var exceptions = new List<Exception>();
            using (CorrelationIdProvider.Change(incomingEvent.GetCorrelationId()))
            {
                await TriggerHandlersFromInboxAsync(eventType, eventData, exceptions, inboxConfig);
            }
            if (exceptions.Any())
            {
                ThrowOriginalExceptions(eventType, exceptions);
            }
        }
    }

    public override async Task PublishAsync(Type eventType, object eventData, bool onUnitOfWorkComplete = true, bool useOutbox = true)
    {
        if (chaoAbpEventBusOptions.Value.BasicInfoEnable == false)
        {
            await base.PublishAsync(eventType, eventData, onUnitOfWorkComplete, useOutbox);
            return;
        }
        if (eventData is not IChaoEventEto)
        {
            eventData = chaoEventEtoBuilder.Configure(eventData);
        }

        if (onUnitOfWorkComplete && UnitOfWorkManager.Current != null)
        {
            AddToUnitOfWork(
                UnitOfWorkManager.Current,
                new UnitOfWorkEventRecord(eventType, eventData, EventOrderGenerator.GetNext(), useOutbox)
            );
            return;
        }

        if (useOutbox)
        {
            if (await AddToOutboxAsync(eventType, eventData))
            {
                return;
            }
        }

        if (eventData is IChaoEventEto chaoEventEto && chaoEventEto.Chao == true)
        {
            await TriggerDistributedEventSentAsync(new DistributedEventSent()
            {
                Source = DistributedEventSource.Direct,
                EventName = EventNameAttribute.GetNameOrDefault(eventType),
                EventData = chaoEventEto.GetEventData()
            });

            await TriggerDistributedEventReceivedAsync(new DistributedEventReceived
            {
                Source = DistributedEventSource.Direct,
                EventName = EventNameAttribute.GetNameOrDefault(eventType),
                EventData = chaoEventEto.GetEventData()
            });
        }

        await PublishToEventBusAsync(eventType, eventData);
    }

    protected override async Task TriggerHandlerAsync(IEventHandlerFactory asyncHandlerFactory, Type eventType,
        object eventData, List<Exception> exceptions, InboxConfig? inboxConfig = null)
    {
        using (var eventHandlerWrapper = asyncHandlerFactory.GetHandler())
        {
            try
            {
                var handlerType = eventHandlerWrapper.EventHandler.GetType();

                if (inboxConfig?.HandlerSelector != null &&
                    !inboxConfig.HandlerSelector(handlerType))
                {
                    return;
                }

                if (eventData is IChaoEventEto chaoEventEto && chaoEventEto.Chao == true)
                {
                    using (CurrentTenant.Change(chaoEventEto.TenantId, chaoEventEto.TenantName))
                    {
                        var claims = defaultClaimBuilder.Build(chaoEventEto.Claims);
                        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                        using (currentPrincipalAccessor.Change(claimsPrincipal))
                        {
                            await InvokeEventHandlerAsync(eventHandlerWrapper.EventHandler, chaoEventEto.GetEventData(), eventType);
                        }
                    }
                }
                else
                {
                    using (CurrentTenant.Change(GetEventDataTenantId(eventData)))
                    {
                        await InvokeEventHandlerAsync(eventHandlerWrapper.EventHandler, eventData, eventType);
                    }
                }
            }
            catch (TargetInvocationException ex)
            {
                exceptions.Add(ex.InnerException!);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
    }

    private async Task ProcessEventAsync(IChannel channel, BasicDeliverEventArgs ea)
    {
        var eventName = ea.RoutingKey;
        var eventType = EventTypes.GetOrDefault(eventName);
        if (eventType == null)
        {
            return;
        }

        var newEventType = typeof(ChaoEventEto<>).MakeGenericType(eventType);
        var eventData = Serializer.Deserialize(ea.Body.ToArray(), newEventType);

        if (eventData is not IChaoEventEto chaoEventEto || chaoEventEto.Chao != true)
        {
            eventData = Serializer.Deserialize(ea.Body.ToArray(), eventType);
        }
        var correlationId = ea.BasicProperties.CorrelationId;
        if (await AddToInboxAsync(ea.BasicProperties.MessageId, eventName, eventType, eventData, correlationId))
        {
            return;
        }

        using (CorrelationIdProvider.Change(correlationId))
        {
            await TriggerHandlersDirectAsync(eventType, eventData);
        }
    }
}