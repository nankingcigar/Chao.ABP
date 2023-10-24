using Chao.Abp.Identity.SSO;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;

namespace Chao.Abp.CAS;

[DependsOn(
    typeof(AbpIdentityWebModule),
    typeof(AbpIdentityAspNetCoreModule),
    typeof(ChaoIdentitySSOModule)
    )]
public class ChaoCASModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpApi<ITokenApi>();
    }
}