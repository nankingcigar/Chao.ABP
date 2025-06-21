using Chao.Abp.AuditLogging.Domain.Store;
using Chao.Abp.Ddd.Domain;
using Chao.Abp.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.AuditLogging;
using Volo.Abp.Modularity;

namespace Chao.Abp.AuditLogging.Domain;

[DependsOn(
    typeof(ChaoAbpDddDomainModule),
    typeof(ChaoAbpJsonModule),
    typeof(AbpAuditLoggingDomainModule)
    )]
public class ChaoAbpAuditLoggingDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<AuditingStore, ChaoAuditingStore>());
    }
}