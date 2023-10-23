using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict;

namespace Chao.Abp.OpenIddict.CAS;

[DependsOn(
    typeof(AbpOpenIddictAspNetCoreModule)
    )]
public class ChaoAbpOpenIddictCASModule : AbpModule
{
}