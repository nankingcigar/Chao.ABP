using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Chao.Abp.TestBase;

[DependsOn(
    typeof(AbpTestBaseModule)
)]
public class ChaoAbpTestBaseModule : AbpModule
{
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        var apis = new List<ServiceDescriptor>();
        foreach (var service in context.Services)
        {
            if (service.ServiceType.IsInterface && service.ImplementationFactory != null && service.ImplementationFactory.Method.Module.Name.Contains("WebApiClientCore") == true)
            {
                apis.Add(service);
            }
        }
        foreach (var service in apis)
        {
            context.Services.Replace(ServiceDescriptor.Singleton(service.ServiceType, NSubstitute.Substitute.For(new Type[1] { service.ServiceType }, new object[0] { })));
        }
    }
}