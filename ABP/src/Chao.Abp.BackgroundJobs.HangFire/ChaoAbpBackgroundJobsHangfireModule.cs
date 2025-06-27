using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Modularity;

namespace Chao.Abp.BackgroundJobs.HangFire;

[DependsOn(
    typeof(AbpBackgroundJobsHangfireModule),
    typeof(ChaoAbpBackgroundJobsAbstractionsModule)
    )]
public class ChaoAbpBackgroundJobsHangfireModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<HangfireBackgroundJobManager, ChaoHangfireBackgroundJobManager>());
    }
}