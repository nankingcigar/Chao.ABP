using Chao.Abp.EventBus.Abstractions;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;

namespace Chao.Abp.EventBus.RabbitMQ;

[DependsOn(
    typeof(ChaoAbpEventBusAbstractionsModule),
    typeof(AbpEventBusRabbitMqModule)
    )]
public class ChaoAbpEventBusRabbitMqModule : AbpModule
{
}