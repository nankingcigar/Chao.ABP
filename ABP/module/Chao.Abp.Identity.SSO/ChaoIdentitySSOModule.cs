using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Modularity;

namespace Chao.Abp.Identity.SSO;

[DependsOn(
    typeof(AbpIdentityAspNetCoreModule)
    )]
public class ChaoIdentitySSOModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpIdentityOptions>(options =>
        {
            options.ExternalLoginProviders.Add(nameof(SSOLoginProvider), new ExternalLoginProviderInfo(nameof(SSOLoginProvider), typeof(SSOLoginProvider)));
        });
    }
}