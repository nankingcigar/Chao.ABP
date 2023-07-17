/*
 * @Author: Chao Yang
 * @Date: 2020-11-18 02:09:05
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-18 02:21:34
 */

using System;
using Volo.Abp.Timing;

namespace Chao.Abp.Timing;

public interface IChaoClock : IClock
{
    DateTime Genesis { get; }
}