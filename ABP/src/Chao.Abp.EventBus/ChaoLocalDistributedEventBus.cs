using Chao.Abp.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Timing;
using Volo.Abp.Tracing;
using Volo.Abp.Uow;

namespace Chao.Abp.EventBus;

[Dependency(ReplaceServices = true)]
public class ChaoLocalDistributedEventBus(IServiceScopeFactory serviceScopeFactory, ICurrentTenant currentTenant, IUnitOfWorkManager unitOfWorkManager, IOptions<AbpDistributedEventBusOptions> abpDistributedEventBusOptions, IGuidGenerator guidGenerator, IClock clock, IEventHandlerInvoker eventHandlerInvoker, ILocalEventBus localEventBus, ICorrelationIdProvider correlationIdProvider, ICurrentPrincipalAccessor currentPrincipalAccessor, IOptions<ChaoAbpEventBusOption> chaoAbpEventBusOptions, ChaoEventEtoBuilder chaoEventEtoBuilder, DefaultClaimBuilder defaultClaimBuilder) : LocalDistributedEventBus(serviceScopeFactory, currentTenant, unitOfWorkManager, abpDistributedEventBusOptions, guidGenerator, clock, eventHandlerInvoker, localEventBus, correlationIdProvider)
{
    public override async Task ProcessFromInboxAsync(IncomingEventInfo incomingEvent, InboxConfig inboxConfig)
    {
        var eventType = EventTypes.GetOrDefault(incomingEvent.EventName);
        if (eventType == null)
        {
            return;
        }
        var newEventType = typeof(ChaoEventEto<>).MakeGenericType(eventType);
        var eventData = JsonSerializer.Deserialize(incomingEvent.EventData, newEventType);

        if (eventData is not IChaoEventEto chaoEventEto || chaoEventEto.Chao != true)
        {
            eventData = JsonSerializer.Deserialize(Encoding.UTF8.GetString(incomingEvent.EventData), eventType)!;
        }
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

        if (eventData is IChaoEventEto ChaoEventEto && ChaoEventEto.Chao == true)
        {
            await TriggerDistributedEventSentAsync(new DistributedEventSent()
            {
                Source = DistributedEventSource.Direct,
                EventName = EventNameAttribute.GetNameOrDefault(eventType),
                EventData = ChaoEventEto.GetEventData()
            });

            await TriggerDistributedEventReceivedAsync(new DistributedEventReceived
            {
                Source = DistributedEventSource.Direct,
                EventName = EventNameAttribute.GetNameOrDefault(eventType),
                EventData = ChaoEventEto.GetEventData()
            });
        }

        await PublishToEventBusAsync(eventType, eventData);
    }

    public override async Task PublishFromOutboxAsync(OutgoingEventInfo outgoingEvent, OutboxConfig outboxConfig)
    {
        await TriggerDistributedEventSentAsync(new DistributedEventSent()
        {
            Source = DistributedEventSource.Outbox,
            EventName = outgoingEvent.EventName,
            EventData = outgoingEvent.EventData
        });

        await TriggerDistributedEventReceivedAsync(new DistributedEventReceived
        {
            Source = DistributedEventSource.Direct,
            EventName = outgoingEvent.EventName,
            EventData = outgoingEvent.EventData
        });

        var eventType = EventTypes.GetOrDefault(outgoingEvent.EventName);
        if (eventType == null)
        {
            return;
        }
        var newEventType = typeof(ChaoEventEto<>).MakeGenericType(eventType);
        var eventData = JsonSerializer.Deserialize(outgoingEvent.EventData, newEventType);

        if (eventData is not IChaoEventEto chaoEventEto || chaoEventEto.Chao != true)
        {
            eventData = JsonSerializer.Deserialize(Encoding.UTF8.GetString(outgoingEvent.EventData), eventType)!;
        }
        if (await AddToInboxAsync(Guid.NewGuid().ToString(), outgoingEvent.EventName, eventType, eventData, null))
        {
            return;
        }
        await LocalEventBus.PublishAsync(eventType, eventData, false);
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
}