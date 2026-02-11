using Chao.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace Chao.Abp.Identity.Application;

[DependsOn(
    typeof(AbpIdentityApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpIdentityApplicationModule : AbpModule
{
}