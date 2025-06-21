using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;

namespace Chao.Abp.AuditLogging.EntityFrameworkCore;

public interface IChaoAuditLoggingDbContext : IAuditLoggingDbContext
{
    DbSet<AuditLogAction> AuditLogAction { get; }
    DbSet<EntityChange> EntityChange { get; }
    DbSet<EntityPropertyChange> EntityPropertyChange { get; }
}