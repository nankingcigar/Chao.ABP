/*
 * @Author: Chao Yang
 * @Date: 2020-11-18 02:10:42
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-18 02:18:14
 */

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