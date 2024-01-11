using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;

namespace Chao.Abp.AspNetCore.Mvc.UI.Components.Shared.Injector;

public class InjectorComponent : AbpViewComponent
{
    public virtual ChaoAbpMvcUiOptions ChaoAbpMvcUiOptions => ChaoAbpMvcUiOptionsOptions!.Value;
    public virtual IOptions<ChaoAbpMvcUiOptions>? ChaoAbpMvcUiOptionsOptions { get; set; }

    public IViewComponentResult Invoke()
    {
        return View("/Components/Shared/Injector/Default.cshtml", ChaoAbpMvcUiOptions);
    }
}