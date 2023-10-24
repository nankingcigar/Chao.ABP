using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;

namespace Chao.Abp.AspNetCore.Mvc.AntiForgery;

[Route("api/abp/antiforgery")]
public class AbpAntiForgeryController : AbpControllerBase
{
    public virtual IAbpAntiForgeryManager AbpAntiForgeryManager { get; set; }

    [IgnoreAntiforgeryToken]
    [HttpPost, Route("")]
    public virtual string Get()
    {
        var token = AbpAntiForgeryManager.GenerateToken();
        return token;
    }
}