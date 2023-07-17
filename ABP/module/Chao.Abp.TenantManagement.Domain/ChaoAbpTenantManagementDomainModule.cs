using Chao.Abp.Ddd.Domain;
using Chao.Abp.MultiTenancy;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace Chao.Abp.TenantManagement.Domain;

[DependsOn(
    typeof(ChaoAbpMultiTenancyModule),
    typeof(ChaoAbpDddDomainModule),
    typeof(AbpTenantManagementDomainModule)
    )]
public class ChaoAbpTenantManagementDomainModule : AbpModule
{
}