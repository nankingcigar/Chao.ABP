using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Chao.Abp.AspNetCore.Mvc.Authorization;

public class ChaoAuthorizeFilter : IAsyncAuthorizationFilter, ITransientDependency
{
    public ChaoAuthorizeFilter(
        IAuthorizationService authorizationService
        , IOptions<ChaoAbpAspNetCoreMvcAuthorizeOption> options
        , IAuthorizationPolicyProvider policyProvider
        )
    {
        AuthorizationService = authorizationService;
        Option = options.Value;
        PolicyProvider = policyProvider;
    }

    public virtual bool And { get; set; }
    public virtual string AuthenticationSchemes { get; set; }
    public virtual IAuthorizationService AuthorizationService { get; set; }
    public virtual ChaoAbpAspNetCoreMvcAuthorizeOption Option { get; set; }
    public virtual IAuthorizationPolicyProvider PolicyProvider { get; set; }
    public virtual string[] Policys { get; set; }

    public virtual async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.Any(item => item is IAllowAnonymous))
        {
            return;
        }
        if (Policys == null || Policys.Length == 0)
        {
            var defaultPolicy = await PolicyProvider.GetDefaultPolicyAsync();
            var authorized = await AuthorizationService.AuthorizeAsync(context.HttpContext.User, null, defaultPolicy);
            if (authorized.Succeeded == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            return;
        }
        if (AuthenticationSchemes.IsNullOrEmpty() == true)
        {
            AuthenticationSchemes = Option.AuthenticationSchemes;
        }
        if (And == false)
        {
            foreach (var policy in Policys)
            {
                var authorized = await AuthorizationService.AuthorizeAsync(context.HttpContext.User, policy);
                if (authorized.Succeeded == true)
                {
                    return;
                }
            }
            context.Result = AuthenticationSchemes.IsNullOrEmpty() ? new ForbidResult() : new ForbidResult(AuthenticationSchemes);
            return;
        }
        else
        {
            foreach (var policy in Policys)
            {
                var authorized = await AuthorizationService.AuthorizeAsync(context.HttpContext.User, policy);
                if (authorized.Succeeded == false)
                {
                    context.Result = AuthenticationSchemes.IsNullOrEmpty() ? new ForbidResult() : new ForbidResult(AuthenticationSchemes);
                    return;
                }
            }
        }
    }
}