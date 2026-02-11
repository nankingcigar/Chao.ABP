using Chao.Abp.AutoMapper;
using Volo.Abp.Account;
using Volo.Abp.Modularity;

namespace Chao.Abp.Account.Application;

[DependsOn(
    typeof(AbpAccountApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpAccountApplicationModule : AbpModule
{
}