using Chao.CAS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;

namespace Chao.Abp.OpenIddict.CAS;

public class OpenIddictCASController : AbpController
{
    public virtual CASHandler CASHandler { get; set; }
    public virtual ChaoAbpOpenIddictCASOption ChaoAbpOpenIddictCASOption => ChaoAbpOpenIddictCASOptions.Value;
    public virtual IOptions<ChaoAbpOpenIddictCASOption> ChaoAbpOpenIddictCASOptions { get; set; }
    public virtual IdentitySecurityLogManager IdentitySecurityLogManager { get; set; }
    public virtual AbpOpenIddictClaimsPrincipalManager OpenIddictClaimsPrincipalManager { get; set; }
    public virtual IOpenIddictScopeManager ScopeManager { get; set; }
    public virtual SignInManager<Volo.Abp.Identity.IdentityUser> SignInManager { get; set; }
    public virtual IdentityUserManager UserManager { get; set; }

    public async Task<IActionResult> Index(string ticket)
    {
        var profile = await CASHandler.GetProfile(ticket);
        var userName = profile?.UserInfo?.UserName;
        if (userName.IsNullOrEmpty() == false)
        {
            var user = await UserManager.FindByNameAsync(userName);
            var principal = await SignInManager.CreateUserPrincipalAsync(user);
            principal.SetScopes(ChaoAbpOpenIddictCASOption.ClientScopes.ToImmutableArray());
            principal.SetResources(await GetResourcesAsync(ChaoAbpOpenIddictCASOption.ClientScopes.ToImmutableArray()));
            await OpenIddictClaimsPrincipalManager.HandleAsync(null, principal);
            var signinResult = SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        ViewData["LandingUri"] = ChaoAbpOpenIddictCASOption.LandingUri;
        return View();
    }

    protected virtual async Task<IEnumerable<string>> GetResourcesAsync(ImmutableArray<string> scopes)
    {
        var resources = new List<string>();
        if (!scopes.Any())
        {
            return resources;
        }

        await foreach (var resource in ScopeManager.ListResourcesAsync(scopes))
        {
            resources.Add(resource);
        }
        return resources;
    }
}