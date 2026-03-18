using AutoMapper;
using AutoMapper.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Chao.Abp.AutoMapper;

[DependsOn(
    typeof(AbpAutoMapperModule)
    )]
public class ChaoAbpAutoMapperModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IConfigurationProvider>(sp =>
        {
            using (var scope = sp.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<AbpAutoMapperOptions>>().Value;

                var mapperConfigurationExpression = sp.GetRequiredService<IOptions<MapperConfigurationExpression>>().Value;
                var autoMapperConfigurationContext = new AbpAutoMapperConfigurationContext(mapperConfigurationExpression, scope.ServiceProvider);

                foreach (var configurator in options.Configurators)
                {
                    configurator(autoMapperConfigurationContext);
                }

                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

                var mapperConfiguration = new MapperConfiguration(mapperConfigurationExpression, loggerFactory);

                foreach (var profileType in options.ValidatingProfiles)
                {
                    mapperConfiguration.Internal().AssertConfigurationIsValid(((Profile)Activator.CreateInstance(profileType)!).ProfileName);
                }

                return mapperConfiguration;
            }
        });
    }
}