/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 03:42:01
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 07:32:04
 */

using Chao.Abp.AspNetCore.Mvc.ExceptionHandling;
using Chao.Abp.AspNetCore.Mvc.ModelBinding;
using Chao.Abp.AspNetCore.Mvc.ResultHandler;
using Chao.Abp.Json;
using Chao.Abp.Json.SystemTextJson.JsonConverters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.ModelBinding;
using Volo.Abp.Modularity;

namespace Chao.Abp.AspNetCore.Mvc;

[DependsOn(
    typeof(ChaoAbpAspNetCoreModule),
    typeof(ChaoAbpJsonSystemTextJsonModule),
    typeof(AbpAspNetCoreMvcModule)
    )]
public class ChaoAbpAspNetCoreMvcModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<AbpExceptionFilter, ChaoAbpExceptionFilter>());
        Configure<MvcOptions>(mvcOptions =>
        {
            var mb = mvcOptions.ModelBinderProviders.First(mb => mb.GetType() == typeof(AbpDateTimeModelBinderProvider));
            var mbIndex = mvcOptions.ModelBinderProviders.IndexOf(mb);
            mvcOptions.ModelBinderProviders[mbIndex] = new ChaoAbpDateTimeModelBinderProvider();
            mvcOptions.Filters.Add(typeof(ChaoResultFilter));
        });
        context.Services.AddOptions<JsonOptions>()
            .Configure<IServiceProvider>((options, rootServiceProvider) =>
            {
                options.JsonSerializerOptions.Converters.Add(rootServiceProvider
                    .GetRequiredService<ChaoAbpDateTimeConverter>());
                options.JsonSerializerOptions.Converters.Add(rootServiceProvider
                    .GetRequiredService<ChaoAbpNullableDateTimeConverter>());
                options.JsonSerializerOptions.Converters.Add(rootServiceProvider
                    .GetRequiredService<ChaoDateTimeOffsetConverter>());
                options.JsonSerializerOptions.Converters.Add(rootServiceProvider
                    .GetRequiredService<ChaoNullableDateTimeOffsetConverter>());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
    }
}