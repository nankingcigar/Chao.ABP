using Chao.Abp.AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;

namespace Chao.Abp.FeatureManagement.Application;

[DependsOn(
    typeof(AbpFeatureManagementApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpFeatureManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChaoAbpFeatureManagementApplicationModule>();
        });
    }
}