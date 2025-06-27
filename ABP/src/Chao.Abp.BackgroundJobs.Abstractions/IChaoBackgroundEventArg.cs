using System;
using System.Collections.Generic;

namespace Chao.Abp.BackgroundJobs.Abstractions;

public interface IChaoBackgroundEventArg
{
    bool Chao { get; set; }
    Dictionary<string, string> Claims { get; set; }
    Guid? TenantId { get; set; }
    string? TenantName { get; set; }

    object GetArg();
}