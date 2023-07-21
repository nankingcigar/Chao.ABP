namespace Chao.Abp.AspNetCore.Auditing;

public class ChaoAbpAspNetCoreAuditingOptions
{
    public ChaoAbpAspNetCoreAuditingOptions()
    {
        DisableAuditClientName = false;
        DnsResolverTimeoutMillisecond = 5;
    }

    public virtual bool DisableAuditClientName { get; set; }
    public virtual int DnsResolverTimeoutMillisecond { get; set; }
}