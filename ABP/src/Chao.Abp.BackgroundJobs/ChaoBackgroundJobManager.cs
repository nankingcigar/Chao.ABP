using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Timing;

namespace Chao.Abp.BackgroundJobs;

[Dependency(ReplaceServices = true)]
public class ChaoBackgroundJobManager(IClock clock, IBackgroundJobSerializer serializer, IBackgroundJobStore store, IGuidGenerator guidGenerator, IOptions<AbpBackgroundJobWorkerOptions> backgroundJobWorkerOptions, IOptions<ChaoAbpBackgroundJobOption> chaoAbpBackgroundJobOptions, ChaoBackgroundEventArgBuilder chaoBackgroundEventArgBuilder) : DefaultBackgroundJobManager(clock, serializer, store, guidGenerator, backgroundJobWorkerOptions)
{
    protected override Task<Guid> EnqueueAsync(string jobName, object args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
    {
        if (chaoAbpBackgroundJobOptions.Value.BasicInfoEnable == false)
        {
            return base.EnqueueAsync(jobName, args, priority, delay);
        }

        var newArg = chaoBackgroundEventArgBuilder.Configure(args);
        return base.EnqueueAsync(jobName, newArg, priority, delay);
    }
}