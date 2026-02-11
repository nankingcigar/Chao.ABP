using Chao.Abp.AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Chao.Abp.Identity.Application;

[DependsOn(
    typeof(AbpIdentityApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpIdentityApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChaoAbpIdentityApplicationModule>();
        });
    }
}