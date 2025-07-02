using Chao.Abp.EventBus;
using Chao.Abp.ResultHandling;
using Chao.Abp.Timing;
using Volo.Abp.Auditing;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Modularity;

namespace Chao.Abp.Ddd.Domain;

[DependsOn(
    typeof(ChaoAbpTimingModule),
    typeof(ChaoAbpResultHandlingModule),
    typeof(AbpDddDomainModule),
    typeof(ChaoAbpEventBusModule)
    )]
public class ChaoAbpDddDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpEntityChangeOptions>(options =>
        {
            options.PublishEntityUpdatedEventWhenNavigationChanges = false;
        });
        Configure<AbpAuditingOptions>(options =>
        {
            options.SaveEntityHistoryWhenNavigationChanges = false;
        });
    }
}