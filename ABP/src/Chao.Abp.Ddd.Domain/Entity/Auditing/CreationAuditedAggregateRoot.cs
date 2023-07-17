using System;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Chao.Abp.Ddd.Domain.Entity.Auditing;

[Serializable]
public abstract class CreationAuditedAggregateRoot : AggregateRoot, ICreationAuditedObject
{
    public virtual DateTime CreationTime { get; set; }

    public virtual Guid? CreatorId { get; set; }
}