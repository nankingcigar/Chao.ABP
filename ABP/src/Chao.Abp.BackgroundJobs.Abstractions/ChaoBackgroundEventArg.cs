using System;
using System.Collections.Generic;

namespace Chao.Abp.BackgroundJobs.Abstractions;

public class ChaoBackgroundEventArg<TArg> : IChaoBackgroundEventArg
{
    public virtual TArg? Arg { get; set; }
    public virtual bool Chao { get; set; }
    public virtual Dictionary<string, string> Claims { get; set; } = [];
    public virtual Guid? TenantId { get; set; }
    public virtual string? TenantName { get; set; }

    public virtual object GetArg()
    {
        return Arg!;
    }
}