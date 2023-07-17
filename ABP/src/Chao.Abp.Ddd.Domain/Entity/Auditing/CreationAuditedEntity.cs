using System;
using Volo.Abp.Auditing;

namespace Chao.Abp.Ddd.Domain.Entity.Auditing;

using Volo.Abp.Domain.Entities;

[Serializable]
public abstract class CreationAuditedEntity : Entity, ICreationAuditedObject
{
    public virtual DateTime CreationTime { get; set; }

    public virtual Guid? CreatorId { get; set; }
}