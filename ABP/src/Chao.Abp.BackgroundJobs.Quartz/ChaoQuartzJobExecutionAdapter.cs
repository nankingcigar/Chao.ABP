using Chao.Abp.BackgroundJobs.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.Json;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Chao.Abp.BackgroundJobs.Quartz;

public class ChaoQuartzJobExecutionAdapter<TArg>(IOptions<AbpBackgroundJobOptions> options, IOptions<AbpBackgroundJobQuartzOptions> backgroundJobQuartzOptions, IBackgroundJobExecuter jobExecuter, IServiceScopeFactory serviceScopeFactory, IJsonSerializer jsonSerializer) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var arg = jsonSerializer.Deserialize<ChaoBackgroundEventArg<TArg>>(context.JobDetail.JobDataMap.GetString(nameof(TArg))!);
            var jobType = options.Value.GetJob(typeof(TArg)).JobType;
            var jobContext = new JobExecutionContext(scope.ServiceProvider, jobType, arg.Arg!, cancellationToken: context.CancellationToken);
            try
            {
                var currentTenant = scope.ServiceProvider.GetRequiredService<ICurrentTenant>();
                var currentPrincipalAccessor = scope.ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>();
                using (currentTenant.Change(arg.TenantId, arg.TenantName))
                {
                    var claims = arg.Claims.Select(c => new Claim(c.Key, c.Value)).ToArray();
                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                    using (currentPrincipalAccessor.Change(claimsPrincipal))
                    {
                        await jobExecuter.ExecuteAsync(jobContext);
                    }
                }
            }
            catch (Exception exception)
            {
                var jobExecutionException = new JobExecutionException(exception);

                var retryIndex = context.JobDetail.JobDataMap.GetString(QuartzBackgroundJobManager.JobDataPrefix + QuartzBackgroundJobManager.RetryIndex)!.To<int>();
                retryIndex++;
                context.JobDetail.JobDataMap.Put(QuartzBackgroundJobManager.JobDataPrefix + QuartzBackgroundJobManager.RetryIndex, retryIndex.ToString());

                await backgroundJobQuartzOptions.Value.RetryStrategy.Invoke(retryIndex, context, jobExecutionException);

                throw jobExecutionException;
            }
        }
    }
}