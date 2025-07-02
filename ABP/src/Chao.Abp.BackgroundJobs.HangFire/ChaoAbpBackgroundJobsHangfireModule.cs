using Chao.Abp.BackgroundJobs.Abstractions;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Modularity;

namespace Chao.Abp.BackgroundJobs.HangFire;

[DependsOn(
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(ChaoAbpBackgroundJobsAbstractionsModule)
    )]
public class ChaoAbpBackgroundJobsHangfireModule : AbpModule
{
}