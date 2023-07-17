namespace Chao.Abp.AspNetCore.Mvc.UI;

public class ChaoAbpMvcUiOptions
{
    public virtual bool EnableInjector { get; set; } = false;
    public virtual string InjectorUri { get; set; } = "/";
}