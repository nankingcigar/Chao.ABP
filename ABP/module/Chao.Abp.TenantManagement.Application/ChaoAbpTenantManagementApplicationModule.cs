using Chao.Abp.AutoMapper;
using Chao.Abp.TenantManagement.Domain;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace Chao.Abp.TenantManagement.Application;

[DependsOn(
    typeof(AbpTenantManagementApplicationModule),
    typeof(ChaoAbpTenantManagementDomainModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpTenantManagementApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChaoAbpTenantManagementApplicationModule>();
        });
    }
}