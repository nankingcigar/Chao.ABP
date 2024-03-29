﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace Chao.Abp.Timing;

[DependsOn(
    typeof(AbpTimingModule)
    )]
public class ChaoAbpTimingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<IClock, ChaoClock>());
        context.Services.Replace(ServiceDescriptor.Transient<Clock, ChaoClock>());
    }
}