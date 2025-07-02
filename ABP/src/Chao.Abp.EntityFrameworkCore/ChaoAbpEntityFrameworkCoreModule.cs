using Chao.Abp.Ddd.Domain;
using Chao.Abp.EntityFrameworkCore.ChangeTrackers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.ChangeTrackers;
using Volo.Abp.Modularity;

namespace Chao.Abp.EntityFrameworkCore;

[DependsOn(
    typeof(ChaoAbpDddDomainModule),
    typeof(AbpEntityFrameworkCoreModule)
    )]
public class ChaoAbpEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<AbpEfCoreNavigationHelper, ChaoAbpEfCoreNavigationHelper>());
    }
}