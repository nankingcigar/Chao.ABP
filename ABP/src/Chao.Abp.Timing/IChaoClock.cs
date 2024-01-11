using System;
using Volo.Abp.Timing;

namespace Chao.Abp.Timing;

public interface IChaoClock : IClock
{
    DateTime Genesis { get; }
}