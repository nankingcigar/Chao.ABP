using Chao.Abp.EventBus.Abstractions;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace Chao.Abp.EventBus;

[DependsOn(
    typeof(ChaoAbpEventBusAbstractionsModule),
    typeof(AbpEventBusModule)
    )]
public class ChaoAbpEventBusModule : AbpModule
{
}