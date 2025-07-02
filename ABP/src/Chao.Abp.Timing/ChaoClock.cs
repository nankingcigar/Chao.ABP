using Microsoft.Extensions.Options;
using System;
using Volo.Abp.Timing;

namespace Chao.Abp.Timing;

public class ChaoClock(
    IOptions<AbpClockOptions> options,
    ICurrentTimezoneProvider currentTimezoneProvider,
    ITimezoneProvider timezoneProvider) : Clock(options, currentTimezoneProvider, timezoneProvider), IChaoClock
{
    public virtual DateTime Genesis => new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
}