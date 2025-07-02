using Volo.Abp.EventBus.Abstractions;
using Volo.Abp.Modularity;

namespace Chao.Abp.EventBus.Abstractions;

[DependsOn(
    typeof(AbpEventBusAbstractionsModule)
)]
public class ChaoAbpEventBusAbstractionsModule : AbpModule
{
}