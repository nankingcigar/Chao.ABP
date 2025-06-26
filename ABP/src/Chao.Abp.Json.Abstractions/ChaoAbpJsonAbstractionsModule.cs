using Volo.Abp.Json;
using Volo.Abp.Modularity;

namespace Chao.Abp.Json.Abstractions;

[DependsOn(
    typeof(AbpJsonAbstractionsModule)
    )]
public class ChaoAbpJsonAbstractionsModule : AbpModule
{
}