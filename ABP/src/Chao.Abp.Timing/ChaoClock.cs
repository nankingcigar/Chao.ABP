using Microsoft.Extensions.Options;
using System;
using Volo.Abp.Timing;

namespace Chao.Abp.Timing;

public class ChaoClock(
    IOptions<AbpClockOptions> options) : Clock(options), IChaoClock
{
    public virtual DateTime Genesis => new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}