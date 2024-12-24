using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using WebApiClientCore;

namespace Chao.Abp.WebApiClient;

[DependsOn(
    typeof(AbpCachingModule)
    )]
public class ChaoAbpWebApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IResponseCacheProvider, ChaoResponseCacheProvider>();
    }
}