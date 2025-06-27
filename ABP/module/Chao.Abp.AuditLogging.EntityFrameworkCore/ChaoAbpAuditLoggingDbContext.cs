using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace Chao.Abp.AuditLogging.EntityFrameworkCore;

[ConnectionStringName(AbpAuditLoggingDbProperties.ConnectionStringName)]
public class ChaoAbpAuditLoggingDbContext(DbContextOptions<ChaoAbpAuditLoggingDbContext> options) : AbpDbContext<ChaoAbpAuditLoggingDbContext>(options), IChaoAuditLoggingDbContext
{
    public virtual DbSet<AuditLogAction> AuditLogAction { get; set; }
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<EntityChange> EntityChange { get; set; }
    public virtual DbSet<EntityPropertyChange> EntityPropertyChange { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ConfigureAuditLogging();
    }
}