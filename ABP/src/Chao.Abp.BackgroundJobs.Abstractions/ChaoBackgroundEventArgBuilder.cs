using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Chao.Abp.BackgroundJobs.Abstractions;

public class ChaoBackgroundEventArgBuilder(IOptions<ChaoAbpBackgroundJobOption> chaoAbpBackgroundJobOptions, ICurrentTenant currentTenant, ICurrentPrincipalAccessor currentPrincipalAccessor) : ITransientDependency
{
    public virtual ChaoBackgroundEventArg<TArgs> Configure<TArgs>(TArgs args)
    {
        var newArg = new ChaoBackgroundEventArg<TArgs>()
        {
            TenantId = currentTenant.Id,
            TenantName = currentTenant.Name,
            Arg = args,
            Chao = true
        };

        if (currentPrincipalAccessor.Principal != null)
        {
            foreach (var claim in currentPrincipalAccessor.Principal.Claims)
            {
                newArg.Claims.Add(claim.Type, claim.Value);
            }
        }

        if (chaoAbpBackgroundJobOptions.Value.DefaultConfigureClaim != null)
        {
            chaoAbpBackgroundJobOptions.Value.DefaultConfigureClaim(newArg.Claims);
        }

        if (chaoAbpBackgroundJobOptions.Value.ArgTypeConfigureClaim.ContainsKey(typeof(TArgs)) == true)
        {
            chaoAbpBackgroundJobOptions.Value.ArgTypeConfigureClaim[typeof(TArgs)](newArg.Claims);
        }

        return newArg;
    }
}