using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.Modularity;

namespace Chao.Abp.BackgroundJobs.RabbitMQ;

[DependsOn(
    typeof(AbpBackgroundJobsRabbitMqModule),
    typeof(ChaoAbpBackgroundJobsAbstractionsModule)
    )]
public class ChaoAbpBackgroundJobsRabbitMqModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Singleton(typeof(IJobQueue<>), typeof(ChaoJobQueue<>)));
    }
}