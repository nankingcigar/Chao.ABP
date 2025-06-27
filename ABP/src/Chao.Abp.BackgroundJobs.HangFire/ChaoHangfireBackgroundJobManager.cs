using Chao.Abp.BackgroundJobs.Abstractions;
using Hangfire;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Hangfire;

namespace Chao.Abp.BackgroundJobs.HangFire;

public class ChaoHangfireBackgroundJobManager(IOptions<AbpBackgroundJobOptions> options, IOptions<ChaoAbpBackgroundJobOption> chaoAbpBackgroundJobOptions, ChaoBackgroundEventArgBuilder chaoBackgroundEventArgBuilder) : HangfireBackgroundJobManager(options)
{
    public override Task<string> EnqueueAsync<TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
    {
        if (chaoAbpBackgroundJobOptions.Value.BasicInfoEnable == false)
        {
            return base.EnqueueAsync(args, priority, delay);
        }

        var newArg = chaoBackgroundEventArgBuilder.Configure(args);

        return Task.FromResult(delay.HasValue
            ? BackgroundJob.Schedule<ChaoHangfireJobExecutionAdapter<TArgs>>(
                adapter => adapter.ExecuteAsync(GetQueueName(typeof(TArgs)), newArg, default),
                delay.Value
            )
            : BackgroundJob.Enqueue<ChaoHangfireJobExecutionAdapter<TArgs>>(
                adapter => adapter.ExecuteAsync(GetQueueName(typeof(TArgs)), newArg, default)
            ));
    }
}