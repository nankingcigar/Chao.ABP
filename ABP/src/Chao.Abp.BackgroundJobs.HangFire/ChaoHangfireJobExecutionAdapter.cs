using Chao.Abp.BackgroundJobs.Abstractions;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;

namespace Chao.Abp.BackgroundJobs.HangFire;

public class ChaoHangfireJobExecutionAdapter<TArgs>(
    IOptions<AbpBackgroundJobOptions> options,
    IBackgroundJobExecuter jobExecuter,
    IServiceScopeFactory serviceScopeFactory)
{
    [Queue("{0}")]
    public async Task ExecuteAsync(string queue, ChaoBackgroundEventArg<TArgs> args, CancellationToken cancellationToken = default)
    {
        if (!options.Value.IsJobExecutionEnabled)
        {
            throw new AbpException(
                "Background job execution is disabled. " +
                "This method should not be called! " +
                "If you want to enable the background job execution, " +
                $"set {nameof(AbpBackgroundJobOptions)}.{nameof(AbpBackgroundJobOptions.IsJobExecutionEnabled)} to true! " +
                "If you've intentionally disabled job execution and this seems a bug, please report it."
            );
        }

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var currentTenant = scope.ServiceProvider.GetRequiredService<ICurrentTenant>();
            var currentPrincipalAccessor = scope.ServiceProvider.GetRequiredService<ICurrentPrincipalAccessor>();
            using (currentTenant.Change(args.TenantId, args.TenantName))
            {
                var claims = args.Claims.Select(c => new Claim(c.Key, c.Value)).ToArray();
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));
                using (currentPrincipalAccessor.Change(claimsPrincipal))
                {
                    var jobType = options.Value.GetJob(typeof(TArgs)).JobType;
                    var context = new JobExecutionContext(scope.ServiceProvider, jobType, args.Arg!, cancellationToken: cancellationToken);
                    await jobExecuter.ExecuteAsync(context);
                }
            }
        }
    }
}