using System;
using System.Collections.Generic;

namespace Chao.Abp.EventBus.Abstractions;

public class ChaoEventEto<TEventEto> : IChaoEventEto
{
    public virtual bool Chao { get; set; }
    public virtual Dictionary<string, string> Claims { get; set; } = [];
    public virtual TEventEto? EventData { get; set; }
    public virtual Guid? TenantId { get; set; }
    public virtual string? TenantName { get; set; }

    public virtual object GetEventData()
    {
        return EventData!;
    }
}