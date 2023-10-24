using Chao.Abp.CAS;
using Chao.Abp.Identity.SSO;
using Chao.CAS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;

namespace Chao.Abp.Identity.CAS;

public class CASController : AbpController
{
    public virtual CASHandler CASHandler { get; set; }
    public virtual ChaoCASOption ChaoCASOption => ChaoCASOptions.Value;
    public virtual IOptions<ChaoCASOption> ChaoCASOptions { get; set; }
    public virtual ChaoIdentitySSOOption ChaoIdentitySSOOption => ChaoIdentitySSOOptions.Value;
    public virtual IOptions<ChaoIdentitySSOOption> ChaoIdentitySSOOptions { get; set; }
    public virtual IIdentityUserRepository IdentityUserRepository { get; set; }
    public virtual AbpSignInManager SignInManager { get; set; }
    public virtual ITokenApi TokenApi { get; set; }

    public async Task<IActionResult> Cookie(string ticket)
    {
        var profile = await CASHandler.GetProfile(ticket);
        var userName = profile?.UserInfo?.UserName;
        if (userName.IsNullOrEmpty() == false)
        {
            var user = await IdentityUserRepository.FindByNormalizedUserNameAsync(userName.Normalize());
            await SignInManager.SignInAsync(user, isPersistent: false);
        }
        ViewData["LandingUri"] = ChaoCASOption.LandingUri;
        return View();
    }

    public async Task<IActionResult> Token(string ticket)
    {
        var profile = await CASHandler.GetProfile(ticket);
        var userName = profile?.UserInfo?.UserName;
        if (userName.IsNullOrEmpty() == false)
        {
            var token = await TokenApi.Get(ChaoCASOption.TokenUri, CurrentTenant?.Id?.ToString() ?? "", ChaoCASOption.GrantType, ChaoCASOption.Scope, ChaoCASOption.ClientId, userName, ChaoIdentitySSOOption.ProviderName);
            ViewData["access_token"] = token.access_token;
            ViewData["token_type"] = token.token_type;
            ViewData["expires_in"] = token.expires_in;
            ViewData["refresh_token"] = token.refresh_token;
        }
        ViewData["LandingUri"] = ChaoCASOption.LandingUri;
        return View();
    }
}