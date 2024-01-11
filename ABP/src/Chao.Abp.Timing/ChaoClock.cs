using Microsoft.Extensions.Options;
using System;
using Volo.Abp.Timing;

namespace Chao.Abp.Timing;

public class ChaoClock : Clock, IChaoClock
{
    public ChaoClock(IOptions<AbpClockOptions> options)
        : base(options)
    {
    }

    public virtual DateTime Genesis => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}