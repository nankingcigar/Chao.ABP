using Chao.CAS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.OpenIddict;

namespace Chao.Abp.CAS;

public class CASController : AbpController
{
    public virtual CASHandler? CASHandler { get; set; }
    public virtual ChaoCASOption ChaoCASOption => ChaoCASOptions!.Value;
    public virtual IOptions<ChaoCASOption>? ChaoCASOptions { get; set; }
    public virtual IIdentityUserRepository? IdentityUserRepository { get; set; }
    public virtual AbpOpenIddictClaimsPrincipalManager? OpenIdDictClaimsPrincipalManager { get; set; }
    public virtual IOptionsMonitor<OpenIddictServerOptions>? OpenIdDictServerOptions { get; set; }
    public virtual IOpenIddictScopeManager? ScopeManager { get; set; }
    public virtual AbpSignInManager? SignInManager { get; set; }

    public async Task<IActionResult> Cookie(string ticket)
    {
        var profile = await CASHandler!.GetProfile(ticket);
        var userName = profile?.UserInfo?.UserName;
        if (userName.IsNullOrEmpty() == false)
        {
            var user = await IdentityUserRepository!.FindByNormalizedUserNameAsync(userName.Normalize());
            if (user != null)
            {
                await SignInManager!.SignInAsync(user, isPersistent: false);
                ViewData["LandingUri"] = ChaoCASOption.LandingUri;
            }
            else
            {
                if (ChaoCASOption.ErrorUri.IsNullOrEmpty() == false)
                {
                    ViewData["ErrorUri"] = ChaoCASOption.ErrorUri;
                }
                else if (ChaoCASOption.ErrorMessage.IsNullOrEmpty() == false)
                {
                    ViewData["ErrorMessage"] = ChaoCASOption.ErrorMessage;
                }
            }
        }
        else
        {
            if (ChaoCASOption.ErrorUri.IsNullOrEmpty() == false)
            {
                ViewData["ErrorUri"] = ChaoCASOption.ErrorUri;
            }
            else if (ChaoCASOption.ErrorMessage.IsNullOrEmpty() == false)
            {
                ViewData["ErrorMessage"] = ChaoCASOption.ErrorMessage;
            }
        }
        return View();
    }

    public async Task<IActionResult> Token(string ticket)
    {
        var profile = await CASHandler!.GetProfile(ticket);
        var userName = profile?.UserInfo?.UserName;
        if (userName.IsNullOrEmpty() == false)
        {
            var user = await IdentityUserRepository!.FindByNormalizedUserNameAsync(userName.Normalize());
            if (user != null)
            {
                var principal = await SignInManager!.CreateUserPrincipalAsync(user);
                principal.SetScopes(ChaoCASOption.Scope);
                var scopes = new ImmutableArray<string>().AddRange(ChaoCASOption.Scope!);
                var resources = new List<string>();
                await foreach (var resource in ScopeManager!.ListResourcesAsync(scopes))
                {
                    resources.Add(resource);
                }
                principal.SetResources(resources);
                await OpenIdDictClaimsPrincipalManager!.HandleAsync(null, principal);
                var options = OpenIdDictServerOptions!.CurrentValue;
                var claims = new Dictionary<string, object>(StringComparer.Ordinal) { { OpenIddictConstants.Claims.Audience, ChaoCASOption.ClientId! } };
                if (ChaoCASOption.Scope!.Any())
                {
                    claims.Add(OpenIddictConstants.Claims.Scope, string.Join(" ", ChaoCASOption.Scope!));
                }
                claims.Add(OpenIddictConstants.Claims.JwtId, Guid.NewGuid().ToString());
                var transaction = HttpContext.Features.Get<OpenIddictServerAspNetCoreFeature>()!.Transaction!;
                var descriptor = new SecurityTokenDescriptor
                {
                    Claims = claims,
                    EncryptingCredentials = options.DisableAccessTokenEncryption
                        ? null
                        : options.EncryptionCredentials.First(),
                    Expires = DateTimeOffset.Now.UtcDateTime + options.AccessTokenLifetime,
                    IssuedAt = DateTimeOffset.Now.UtcDateTime,
                    Issuer = transaction.BaseUri!.AbsoluteUri,
                    SigningCredentials = options.SigningCredentials.First(),
                    Subject = (ClaimsIdentity)principal.Identity!,
                    TokenType = OpenIddictConstants.JsonWebTokenTypes.AccessToken,
                };
                var accessToken = options.JsonWebTokenHandler.CreateToken(descriptor);
                ViewData["access_token"] = accessToken;
                ViewData["token_type"] = OpenIddictConstants.Schemes.Bearer;
                ViewData["expires_in"] = descriptor.Expires;
                ViewData["LandingUri"] = ChaoCASOption.LandingUri;
            }
            else
            {
                if (ChaoCASOption.ErrorUri.IsNullOrEmpty() == false)
                {
                    ViewData["ErrorUri"] = ChaoCASOption.ErrorUri;
                }
                else if (ChaoCASOption.ErrorMessage.IsNullOrEmpty() == false)
                {
                    ViewData["ErrorMessage"] = ChaoCASOption.ErrorMessage;
                }
            }
        }
        else
        {
            if (ChaoCASOption.ErrorUri.IsNullOrEmpty() == false)
            {
                ViewData["ErrorUri"] = ChaoCASOption.ErrorUri;
            }
            else if (ChaoCASOption.ErrorMessage.IsNullOrEmpty() == false)
            {
                ViewData["ErrorMessage"] = ChaoCASOption.ErrorMessage;
            }
        }
        return View();
    }
}