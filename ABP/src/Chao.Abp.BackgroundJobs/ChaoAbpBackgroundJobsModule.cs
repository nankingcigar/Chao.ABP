using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Chao.Abp.BackgroundJobs;

[DependsOn(
    typeof(AbpBackgroundJobsModule),
    typeof(ChaoAbpBackgroundJobsAbstractionsModule)
    )]
public class ChaoAbpBackgroundJobsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<BackgroundJobWorker, ChaoBackgroundJobWorker>());
    }
}