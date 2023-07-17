using Chao.Abp.AspNetCore.WebClientInfo;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;

namespace Chao.Abp.AspNetCore.Auditing;

public class AspNetCoreAuditLogContributor : AuditLogContributor, ITransientDependency
{
    public override void PreContribute(AuditLogContributionContext context)
    {
        var clientInfoProvider = context.ServiceProvider.GetRequiredService<IWebClientInfoProvider>();
        if (context.AuditInfo.ClientName == null)
        {
            context.AuditInfo.ClientName = clientInfoProvider.GetComputerName();
        }
    }
}