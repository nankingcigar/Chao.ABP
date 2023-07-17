/*
 * @Author: Chao Yang
 * @Date: 2020-11-24 02:25:36
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-24 03:06:52
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Modularity;

namespace Chao.Abp.ExceptionHandling;

[DependsOn(
    typeof(AbpExceptionHandlingModule)
    )]
public class ChaoAbpExceptionHandlingModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<IExceptionToErrorInfoConverter, ChaoDefaultExceptionToErrorInfoConverter>());
    }
}