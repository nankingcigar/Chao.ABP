using Chao.Abp.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;

namespace Chao.Abp.EventBus;

[Dependency(ReplaceServices = true)]
public class ChaoLocalEventBus(IOptions<AbpLocalEventBusOptions> options, IServiceScopeFactory serviceScopeFactory, ICurrentTenant currentTenant, IUnitOfWorkManager unitOfWorkManager, IEventHandlerInvoker eventHandlerInvoker, ICurrentPrincipalAccessor currentPrincipalAccessor, DefaultClaimBuilder defaultClaimBuilder) : LocalEventBus(options, serviceScopeFactory, currentTenant, unitOfWorkManager, eventHandlerInvoker)
{
    protected override async Task TriggerHandlerAsync(IEventHandlerFactory asyncHandlerFactory, Type eventType, object eventData, List<Exception> exceptions, InboxConfig? inboxConfig = null)
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