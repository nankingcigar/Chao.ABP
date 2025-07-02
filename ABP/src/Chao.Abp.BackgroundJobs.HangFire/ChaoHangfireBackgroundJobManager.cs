using Chao.Abp.BackgroundJobs.Abstractions;
using Hangfire;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Hangfire;

namespace Chao.Abp.BackgroundJobs.HangFire;

[Dependency(ReplaceServices = true)]
public class ChaoHangfireBackgroundJobManager(IOptions<AbpBackgroundJobOptions> backgroundJobOptions, IOptions<AbpHangfireOptions> hangfireOptions, IOptions<ChaoAbpBackgroundJobOption> chaoAbpBackgroundJobOptions, ChaoBackgroundEventArgBuilder chaoBackgroundEventArgBuilder) : HangfireBackgroundJobManager(backgroundJobOptions, hangfireOptions)
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