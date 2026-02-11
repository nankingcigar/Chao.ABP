using Chao.Abp.AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace Chao.Abp.SettingManagement.Application;

[DependsOn(
    typeof(AbpSettingManagementApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpSettingManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChaoAbpSettingManagementApplicationModule>();
        });
    }
}