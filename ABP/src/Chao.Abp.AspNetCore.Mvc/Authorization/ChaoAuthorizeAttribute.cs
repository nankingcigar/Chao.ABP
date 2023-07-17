using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Chao.Abp.AspNetCore.Mvc.Authorization;

public class ChaoAuthorizeAttribute : AuthorizeAttribute, IFilterFactory
{
    public ChaoAuthorizeAttribute()
    {
    }

    public ChaoAuthorizeAttribute(params string[] policys)
    {
        Policys = policys;
    }

    public virtual bool And { get; set; }
    public virtual bool IsReusable => true;
    public virtual string[] Policys { get; set; }

    public virtual IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var chaoAuthorizeFilter = serviceProvider.GetRequiredService<ChaoAuthorizeFilter>();
        chaoAuthorizeFilter.And = And;
        chaoAuthorizeFilter.AuthenticationSchemes = AuthenticationSchemes;
        chaoAuthorizeFilter.Policys = Policys;
        return chaoAuthorizeFilter;
    }
}