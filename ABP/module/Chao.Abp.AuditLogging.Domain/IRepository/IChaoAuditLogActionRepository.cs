using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;

namespace Chao.Abp.AuditLogging.Domain;

public interface IChaoAuditLogActionRepository : IRepository<AuditLogAction>
{
}