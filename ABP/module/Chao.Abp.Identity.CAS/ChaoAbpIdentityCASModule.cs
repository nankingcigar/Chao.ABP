using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.Web;
using Volo.Abp.Modularity;

namespace Chao.Abp.Identity.CAS;

[DependsOn(
    typeof(AbpIdentityWebModule),
    typeof(AbpIdentityAspNetCoreModule)
    )]
public class ChaoAbpIdentityCASModule : AbpModule
{
}