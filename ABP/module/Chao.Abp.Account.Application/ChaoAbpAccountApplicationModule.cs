using Chao.Abp.AutoMapper;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Chao.Abp.Account.Application;

[DependsOn(
    typeof(AbpAccountApplicationModule),
    typeof(ChaoAbpAutoMapperModule)
    )]
public class ChaoAbpAccountApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ChaoAbpAccountApplicationModule>();
        });
    }
}