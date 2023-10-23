using Chao.CAS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;

namespace Chao.Abp.Identity.CAS;

public class IdentityCASController : AbpController
{
    public virtual CASHandler CASHandler { get; set; }
    public virtual ChaoAbpIdentityCASOption ChaoAbpIdentityCASOption => ChaoAbpIdentityCASOptions.Value;
    public virtual IOptions<ChaoAbpIdentityCASOption> ChaoAbpIdentityCASOptions { get; set; }
    public virtual IIdentityUserRepository IdentityUserRepository { get; set; }
    public virtual AbpSignInManager SignInManager { get; set; }

    public async Task<IActionResult> Index(string ticket)
    {
        var profile = await CASHandler.GetProfile(ticket);
        var userName = profile?.UserInfo?.UserName;
        if (userName.IsNullOrEmpty() == false)
        {
            var user = await IdentityUserRepository.FindByNormalizedUserNameAsync(userName.Normalize());
            await SignInManager.SignInAsync(user, isPersistent: false);
        }
        ViewData["LandingUri"] = ChaoAbpIdentityCASOption.LandingUri;
        return View();
    }
}