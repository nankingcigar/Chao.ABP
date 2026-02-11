using Chao.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace Chao.Abp.PermissionManagement.Application;

[DependsOn(
    typeof(AbpPermissionManagementApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpPermissionManagementApplicationModule : AbpModule
{
}