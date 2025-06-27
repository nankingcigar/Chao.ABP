using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.Modularity;

namespace Chao.Abp.BackgroundJobs.Quartz;

[DependsOn(
    typeof(AbpBackgroundJobsQuartzModule),
    typeof(ChaoAbpBackgroundJobsAbstractionsModule)
    )]
public class ChaoAbpBackgroundJobsQuartzModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<QuartzBackgroundJobManager, ChaoQuartzBackgroundJobManager>());
    }
}