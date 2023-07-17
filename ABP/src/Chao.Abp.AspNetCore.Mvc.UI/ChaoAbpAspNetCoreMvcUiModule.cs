using Chao.Abp.AspNetCore.Mvc.UI.Components.Shared.Injector;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.Modularity;
using Volo.Abp.Ui.LayoutHooks;

namespace Chao.Abp.AspNetCore.Mvc.UI;

[DependsOn(
    typeof(ChaoAbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreMvcUiModule)
    )]
public class ChaoAbpAspNetCoreMvcUiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLayoutHookOptions>(options =>
        {
            options.Add(
                LayoutHooks.Head.Last,
                typeof(InjectorComponent)
            );
        });
    }
}