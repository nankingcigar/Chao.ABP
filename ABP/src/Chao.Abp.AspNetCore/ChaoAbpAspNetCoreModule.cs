﻿using Chao.Abp.AspNetCore.ExceptionHandling;
using Chao.Abp.Authorization;
using Chao.Abp.Ddd.Domain;
using Chao.Abp.ResultHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Modularity;

namespace Chao.Abp.AspNetCore.Mvc;

[DependsOn(
    typeof(ChaoAbpAuthorizationModule),
    typeof(ChaoAbpDddDomainModule),
    typeof(ChaoAbpResultHandlingModule),
    typeof(AbpAspNetCoreModule)
    )]
public class ChaoAbpAspNetCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Replace(ServiceDescriptor.Transient<DefaultHttpExceptionStatusCodeFinder, ChaoDefaultHttpExceptionStatusCodeFinder>());
    }
}