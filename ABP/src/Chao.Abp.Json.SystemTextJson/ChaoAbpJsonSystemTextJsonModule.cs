﻿using Chao.Abp.Json.Abstractions;
using Chao.Abp.Json.SystemTextJson.JsonConverters;
using Chao.Abp.Timing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.Modularity;

namespace Chao.Abp.Json;

[DependsOn(
    typeof(ChaoAbpTimingModule),
    typeof(ChaoAbpJsonAbstractionsModule),
    typeof(AbpJsonSystemTextJsonModule)
    )]
public class ChaoAbpJsonSystemTextJsonModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<AbpDateTimeConverter, ChaoAbpDateTimeConverter>());
        context.Services.Replace(ServiceDescriptor.Transient<AbpNullableDateTimeConverter, ChaoAbpNullableDateTimeConverter>());
        context.Services.AddOptions<AbpSystemTextJsonSerializerOptions>()
            .Configure<IServiceProvider>((options, rootServiceProvider) =>
            {
                options.JsonSerializerOptions.Converters.Add(rootServiceProvider
                    .GetRequiredService<ChaoAbpDateTimeConverter>());
                options.JsonSerializerOptions.Converters.Add(rootServiceProvider
                    .GetRequiredService<ChaoAbpNullableDateTimeConverter>());
            });
    }
}