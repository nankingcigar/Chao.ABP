using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;

namespace Chao.Abp.EventBus.RabbitMQ;

[DependsOn(
    typeof(ChaoAbpEventBusModule),
    typeof(AbpEventBusRabbitMqModule)
    )]
public class ChaoAbpEventBusRabbitMqModule : AbpModule
{
}