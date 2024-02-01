using System;
using Volo.Abp.Auditing;

namespace Chao.Abp.Ddd.Domain.Entity.Auditing;

[Serializable]
public abstract class CreationAuditedEntityWithUser<TUser> : CreationAuditedEntity, ICreationAuditedObject<TUser>
{
    public virtual TUser? Creator { get; protected set; }
}