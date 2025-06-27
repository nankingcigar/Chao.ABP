using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Chao.Abp.BackgroundJobs.Abstractions;

[DependsOn(typeof(AbpBackgroundJobsAbstractionsModule))]
public class ChaoAbpBackgroundJobsAbstractionsModule : AbpModule
{
}