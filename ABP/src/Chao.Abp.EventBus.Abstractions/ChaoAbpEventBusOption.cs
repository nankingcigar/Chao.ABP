using System;
using System.Collections.Generic;

namespace Chao.Abp.EventBus.Abstractions;

public class ChaoAbpEventBusOption
{
    public virtual Dictionary<Type, Action<Dictionary<string, string>>> EventTypeConfigureClaim { get; set; } = [];
    public virtual bool BasicInfoEnable { get; set; } = true;
    public virtual Action<Dictionary<string, string>>? DefaultConfigureClaim { get; set; }
}