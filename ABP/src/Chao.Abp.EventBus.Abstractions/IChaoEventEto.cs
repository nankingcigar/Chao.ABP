using System;
using System.Collections.Generic;

namespace Chao.Abp.EventBus.Abstractions;

public interface IChaoEventEto
{
    bool Chao { get; set; }
    Dictionary<string, string> Claims { get; set; }
    Guid? TenantId { get; set; }
    string? TenantName { get; set; }

    object GetEventData();
}