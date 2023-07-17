using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Volo.Abp.AspNetCore.Mvc;

namespace Chao.Abp.AspNetCore.Mvc.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class DisableImpersonatorAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var impersonatorProvider = context.GetRequiredService<IImpersonatorProvider>();
        if (impersonatorProvider.Impersonator == true)
        {
            context.Result = new ForbidResult();
        }
    }
}