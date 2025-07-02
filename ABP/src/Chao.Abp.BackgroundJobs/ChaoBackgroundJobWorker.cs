using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

namespace Chao.Abp.BackgroundJobs;

public class ChaoBackgroundJobWorker(AbpAsyncTimer timer, IOptions<AbpBackgroundJobOptions> jobOptions, IOptions<AbpBackgroundJobWorkerOptions> workerOptions, IServiceScopeFactory serviceScopeFactory, IAbpDistributedLock distributedLock, ICurrentTenant currentTenant, ICurrentPrincipalAccessor currentPrincipalAccessor) : BackgroundJobWorker(timer, jobOptions, workerOptions, serviceScopeFactory, distributedLock)
{
    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        await using (var handler = await DistributedLock.TryAcquireAsync(WorkerOptions.DistributedLockName, cancellationToken: StoppingToken))
        {
            if (handler != null)
            {
                var store = workerContext.ServiceProvider.GetRequiredService<IBackgroundJobStore>();

                var waitingJobs = await store.GetWaitingJobsAsync(WorkerOptions.ApplicationName, WorkerOptions.MaxJobFetchCount);

                if (!waitingJobs.Any())
                {
                    return;
                }

                var jobExecuter = workerContext.ServiceProvider.GetRequiredService<IBackgroundJobExecuter>();
                var clock = workerContext.ServiceProvider.GetRequiredService<IClock>();
                var serializer = workerContext.ServiceProvider.GetRequiredService<IBackgroundJobSerializer>();

                foreach (var jobInfo in waitingJobs)
                {
                    jobInfo.TryCount++;
                    jobInfo.LastTryTime = clock.Now;

                    try
                    {
                        var jobConfiguration = JobOptions.GetJob(jobInfo.JobName);
                        var argsType = jobConfiguration.ArgsType;
                        argsType = typeof(ChaoBackgroundEventArg<>).MakeGenericType(argsType);
                        var jobArgs = serializer.Deserialize(jobInfo.JobArgs, argsType);
                        if (jobArgs is IChaoBackgroundEventArg chaoBackgroundEventArg && chaoBackgroundEventArg.Chao == true)
                        {
                            using (currentTenant.Change(chaoBackgroundEventArg.TenantId, chaoBackgroundEventArg.TenantName))
                            {
                                var claims = chaoBackgroundEventArg.Claims.Select(c => new Claim(c.Key, c.Value)).ToArray();
                                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                                using (currentPrincipalAccessor.Change(claimsPrincipal))
                                {
                                    var context = new JobExecutionContext(
                                        workerContext.ServiceProvider,
                                        jobConfiguration.JobType,
                                        chaoBackgroundEventArg.GetArg(),
                                        workerContext.CancellationToken);

                                    try
                                    {
                                        await jobExecuter.ExecuteAsync(context);

                                        await store.DeleteAsync(jobInfo.Id);
                                    }
                                    catch (BackgroundJobExecutionException)
                                    {
                                        var nextTryTime = CalculateNextTryTime(jobInfo, clock);

                                        if (nextTryTime.HasValue)
                                        {
                                            jobInfo.NextTryTime = nextTryTime.Value;
                                        }
                                        else
                                        {
                                            jobInfo.IsAbandoned = true;
                                        }

                                        await TryUpdateAsync(store, jobInfo);
                                    }
                                }
                            }
                        }
                        else
                        {
                            jobArgs = serializer.Deserialize(jobInfo.JobArgs, jobConfiguration.ArgsType);
                            var context = new JobExecutionContext(
                                workerContext.ServiceProvider,
                                jobConfiguration.JobType,
                                jobArgs,
                                workerContext.CancellationToken);

                            try
                            {
                                await jobExecuter.ExecuteAsync(context);

                                await store.DeleteAsync(jobInfo.Id);
                            }
                            catch (BackgroundJobExecutionException)
                            {
                                var nextTryTime = CalculateNextTryTime(jobInfo, clock);

                                if (nextTryTime.HasValue)
                                {
                                    jobInfo.NextTryTime = nextTryTime.Value;
                                }
                                else
                                {
                                    jobInfo.IsAbandoned = true;
                                }

                                await TryUpdateAsync(store, jobInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex);
                        jobInfo.IsAbandoned = true;
                        await TryUpdateAsync(store, jobInfo);
                    }
                }
            }
            else
            {
                try
                {
                    await Task.Delay(WorkerOptions.JobPollPeriod * 12, StoppingToken);
                }
                catch (TaskCanceledException) { }
            }
        }
    }
}