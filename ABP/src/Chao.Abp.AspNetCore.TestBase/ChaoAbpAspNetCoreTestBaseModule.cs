using Chao.Abp.AspNetCore.Mvc;
using Chao.Abp.TestBase;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.Modularity;

namespace Chao.Abp.AspNetCore.TestBase;

[DependsOn(
    typeof(ChaoAbpAspNetCoreModule),
    typeof(ChaoAbpTestBaseModule),
    typeof(AbpAspNetCoreTestBaseModule)
    )]
public class ChaoAbpAspNetCoreTestBaseModule : AbpModule
{
}