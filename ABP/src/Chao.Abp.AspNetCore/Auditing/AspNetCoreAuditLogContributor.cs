using Chao.Abp.AspNetCore.WebClientInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;

namespace Chao.Abp.AspNetCore.Auditing;

public class AspNetCoreAuditLogContributor : AuditLogContributor, ITransientDependency
{
    public override void PreContribute(AuditLogContributionContext context)
    {
        var clientInfoProvider = context.ServiceProvider.GetRequiredService<IWebClientInfoProvider>();
        var option = context.ServiceProvider.GetRequiredService<IOptions<ChaoAbpAspNetCoreAuditingOptions>>().Value;
        if (context.AuditInfo.ClientName == null)
        {
            context.AuditInfo.ClientName = clientInfoProvider.GetComputerName(option);
        }
    }
}