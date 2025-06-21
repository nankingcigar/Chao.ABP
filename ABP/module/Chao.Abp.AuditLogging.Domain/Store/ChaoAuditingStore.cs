using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Chao.Abp.AuditLogging.Domain.Store;

public class ChaoAuditingStore(IAuditLogRepository auditLogRepository, IChaoAuditLogRepository chaoAuditLogRepository, IChaoAuditLogActionRepository chaoAuditLogActionRepository, IChaoEntityChangeRepository chaoEntityChangeRepository, IChaoEntityPropertyChangeRepository chaoEntityPropertyChangeRepository, IUnitOfWorkManager unitOfWorkManager, IOptions<AbpAuditingOptions> options, IAuditLogInfoToAuditLogConverter converter) : AuditingStore(auditLogRepository, unitOfWorkManager, options, converter)
{
    protected override async Task SaveLogAsync(AuditLogInfo auditInfo)
    {
        using var uow = UnitOfWorkManager.Begin(true);
        var auditLog = await Converter.ConvertAsync(auditInfo);
        IList<AuditLog> auditLogs = [auditLog];
        IList<AuditLogAction> auditLogActions = [.. auditLog.Actions];
        IList<EntityChange> entityChanges = [.. auditLog.EntityChanges];
        IList<EntityPropertyChange> entityPropertyChanges = [.. auditLog.EntityChanges.SelectMany(ec => ec.PropertyChanges)];
        await chaoAuditLogRepository.BulkInsert(auditLogs);
        await chaoAuditLogActionRepository.BulkInsert(auditLogActions);
        if (entityChanges.Any())
            await chaoEntityChangeRepository.BulkInsert(entityChanges);
        if (entityPropertyChanges.Any())
            await chaoEntityPropertyChangeRepository.BulkInsert(entityPropertyChanges);
        await uow.CompleteAsync();
    }
}