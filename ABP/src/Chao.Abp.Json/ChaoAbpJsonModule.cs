using Volo.Abp.Json;
using Volo.Abp.Modularity;

namespace Chao.Abp.Json;

[DependsOn(
    typeof(ChaoAbpJsonSystemTextJsonModule),
    typeof(AbpJsonModule)
    )]
public class ChaoAbpJsonModule : AbpModule
{
}