using Chao.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace Chao.Abp.SettingManagement.Application;

[DependsOn(
    typeof(AbpSettingManagementApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpSettingManagementApplicationModule : AbpModule
{
}