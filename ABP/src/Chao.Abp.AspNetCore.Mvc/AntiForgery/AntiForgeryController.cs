using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Auditing;

namespace Chao.Abp.AspNetCore.Mvc.AntiForgery;

[Area("Abp")]
[DisableAuditing]
[RemoteService(false)]
[ApiExplorerSettings(IgnoreApi = true)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
[ApiController]
[Route("api/antiforgery")]
public class AntiForgeryController : AbpControllerBase
{
    public virtual IAbpAntiForgeryManager? AbpAntiForgeryManager { get; set; }

    [IgnoreAntiforgeryToken]
    [HttpPost, Route("")]
    public virtual string Get()
    {
        var token = AbpAntiForgeryManager!.GenerateToken();
        return token;
    }
}