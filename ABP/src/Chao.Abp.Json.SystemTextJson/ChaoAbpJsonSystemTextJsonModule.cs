/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 01:26:17
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 11:56:55
 */

using Chao.Abp.Json.SystemTextJson.JsonConverters;
using Chao.Abp.Timing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Json.SystemTextJson;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.Modularity;

namespace Chao.Abp.Json;

[DependsOn(
    typeof(ChaoAbpTimingModule),
    typeof(AbpJsonSystemTextJsonModule)
    )]
public class ChaoAbpJsonSystemTextJsonModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<AbpDateTimeConverter, ChaoAbpDateTimeConverter>());
        context.Services.Replace(ServiceDescriptor.Transient<AbpNullableDateTimeConverter, ChaoAbpNullableDateTimeConverter>());
    }
}