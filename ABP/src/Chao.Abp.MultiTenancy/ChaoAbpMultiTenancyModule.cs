using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;

namespace Chao.Abp.MultiTenancy;

[DependsOn(
    typeof(AbpMultiTenancyModule)
    )]
public class ChaoAbpMultiTenancyModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<ICurrentTenant, ChaoCurrentTenant>());
    }
}