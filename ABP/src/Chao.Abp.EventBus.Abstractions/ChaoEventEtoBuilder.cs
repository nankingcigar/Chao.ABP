using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Chao.Abp.EventBus.Abstractions;

public class ChaoEventEtoBuilder(IOptions<ChaoAbpEventBusOption> chaoAbpEventBusOptions, ICurrentTenant currentTenant, ICurrentPrincipalAccessor currentPrincipalAccessor) : ITransientDependency
{
    public virtual ChaoEventEto<TEventData> Configure<TEventData>(TEventData eventData)
    {
        var newArg = new ChaoEventEto<TEventData>()
        {
            TenantId = currentTenant.Id,
            TenantName = currentTenant.Name,
            EventData = eventData,
            Chao = true
        };

        foreach (var claim in currentPrincipalAccessor.Principal.Claims)
        {
            newArg.Claims.Add(claim.Type, claim.Value);
        }

        if (chaoAbpEventBusOptions.Value.DefaultConfigureClaim != null)
        {
            chaoAbpEventBusOptions.Value.DefaultConfigureClaim(newArg.Claims);
        }

        if (chaoAbpEventBusOptions.Value.EventTypeConfigureClaim.ContainsKey(typeof(TEventData)) == true)
        {
            chaoAbpEventBusOptions.Value.EventTypeConfigureClaim[typeof(TEventData)](newArg.Claims);
        }

        return newArg;
    }
}