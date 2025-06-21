using Chao.Abp.AuditLogging.Domain;
using Chao.Abp.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Chao.Abp.AuditLogging.EntityFrameworkCore;

[DependsOn(typeof(AbpAuditLoggingEntityFrameworkCoreModule))]
[DependsOn(typeof(ChaoAbpAuditLoggingDomainModule))]
[DependsOn(typeof(ChaoAbpEntityFrameworkCoreModule))]
public class ChaoAbpAuditLoggingEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddChaoAbpDbContext<ChaoAbpAuditLoggingDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);
        });
    }
}