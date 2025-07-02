using Chao.Abp.BackgroundJobs.Abstractions;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.Modularity;

namespace Chao.Abp.BackgroundJobs.Quartz;

[DependsOn(
    typeof(AbpBackgroundJobsQuartzModule),
    typeof(ChaoAbpBackgroundJobsAbstractionsModule)
    )]
public class ChaoAbpBackgroundJobsQuartzModule : AbpModule
{
}