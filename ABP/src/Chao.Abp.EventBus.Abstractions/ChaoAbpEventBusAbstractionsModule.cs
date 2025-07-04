using Volo.Abp.EventBus.Abstractions;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security;

namespace Chao.Abp.EventBus.Abstractions;

[DependsOn(
    typeof(AbpEventBusAbstractionsModule),
    typeof(AbpMultiTenancyModule),
    typeof(AbpSecurityModule)
)]
public class ChaoAbpEventBusAbstractionsModule : AbpModule
{
}