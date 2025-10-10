using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Chao.Abp.BackgroundJobs.Quartz;

[Dependency(ReplaceServices = true)]
public class ChaoQuartzBackgroundJobManager(IScheduler scheduler, IOptions<AbpBackgroundJobQuartzOptions> options, IJsonSerializer jsonSerializer, IOptions<ChaoAbpBackgroundJobOption> chaoAbpBackgroundJobOptions, ChaoBackgroundEventArgBuilder chaoBackgroundEventArgBuilder) : QuartzBackgroundJobManager(scheduler, options, jsonSerializer)
{
    public override async Task<string> ReEnqueueAsync<TArgs>(TArgs args, int retryCount, int retryIntervalMillisecond, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
    {
        if (chaoAbpBackgroundJobOptions.Value.BasicInfoEnable == false)
        {
            return await base.ReEnqueueAsync<TArgs>(args, retryCount, retryIntervalMillisecond, priority, delay);
        }
        var newArg = chaoBackgroundEventArgBuilder.Configure(args);

        var jobDataMap = new JobDataMap
        {
            {nameof(TArgs), JsonSerializer.Serialize(newArg)},
            {JobDataPrefix+ nameof(Options.RetryCount), retryCount.ToString()},
            {JobDataPrefix+ nameof(Options.RetryIntervalMillisecond), retryIntervalMillisecond.ToString()},
            {JobDataPrefix+ RetryIndex, "0"}
        };

        var jobDetail = JobBuilder.Create<ChaoQuartzJobExecutionAdapter<TArgs>>().RequestRecovery().SetJobData(jobDataMap).Build();
        var trigger = !delay.HasValue ? TriggerBuilder.Create().StartNow().Build() : TriggerBuilder.Create().StartAt(new DateTimeOffset(DateTime.Now.Add(delay.Value))).Build();
        await Scheduler.ScheduleJob(jobDetail, trigger);
        return jobDetail.Key.ToString();
    }
}