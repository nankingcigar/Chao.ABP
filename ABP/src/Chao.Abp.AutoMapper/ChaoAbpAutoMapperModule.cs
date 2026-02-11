using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Chao.Abp.AutoMapper;

[DependsOn(
    typeof(AbpAutoMapperModule)
    )]
public class ChaoAbpAutoMapperModule : AbpModule
{
}