using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Chao.Abp.EntityFrameworkCore;

public class ChaoAbpEnittyFrameworkCoreOption
{
    public virtual bool EnableUpdateManyOptimization { get; set; } = true;

    public virtual IList<string> BasicPropertyNames { get; set; } = [
        nameof(IHasCreationTime.CreationTime),
        nameof(IMayHaveCreator.CreatorId),
        nameof(IMustHaveCreator.CreatorId),
        nameof(IHasModificationTime.LastModificationTime),
        nameof(IModificationAuditedObject.LastModifierId),
        nameof(ISoftDelete.IsDeleted),
        nameof(IHasDeletionTime.DeletionTime),
        nameof(IDeletionAuditedObject.DeleterId),
        nameof(IHasEntityVersion.EntityVersion),
        nameof(IHasConcurrencyStamp.ConcurrencyStamp)
    ];
}