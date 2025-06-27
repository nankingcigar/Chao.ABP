using System;
using System.Collections.Generic;
using System.Text;

namespace Chao.Abp.BackgroundJobs.Abstractions;

public class ChaoAbpBackgroundJobOption
{
    public virtual Dictionary<Type, Action<Dictionary<string, string>>> ArgTypeConfigureClaim { get; set; } = [];
    public virtual bool BasicInfoEnable { get; set; } = true;
    public virtual Action<Dictionary<string, string>>? DefaultConfigureClaim { get; set; }
}