/*
 * @Author: Chao Yang
 * @Date: 2020-11-17 13:34:22
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-17 16:43:53
 */

using Chao.Abp.Authorization.Permission;
using Chao.Abp.MultiTenancy;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;

namespace Chao.Abp.Authorization;

[DependsOn(
    typeof(ChaoAbpMultiTenancyModule),
    typeof(AbpAuthorizationModule)
    )]
public class ChaoAbpAuthorizationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpPermissionOptions>(options =>
        {
            options.ValueProviders.Add<PackPermissionValueProvider>();
        });
        Configure<ChaoAbpPermissionOption>(options =>
        {
            options.PackClaimType = PackPermissionValueProvider.ProviderName;
        });
    }
}