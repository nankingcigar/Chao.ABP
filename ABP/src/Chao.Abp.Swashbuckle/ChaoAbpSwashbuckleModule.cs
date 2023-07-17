using Chao.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.Swashbuckle;

namespace Chao.Abp.Swashbuckle;

[DependsOn(
    typeof(ChaoAbpAspNetCoreMvcModule),
    typeof(AbpSwashbuckleModule)
    )]
public class ChaoAbpSwashbuckleModule : AbpModule
{
}