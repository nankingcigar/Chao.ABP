using Chao.Abp.AutoMapper;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Chao.Abp.PermissionManagement.Application;

[DependsOn(
    typeof(AbpPermissionManagementApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpPermissionManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChaoAbpPermissionManagementApplicationModule>();
        });
    }
}