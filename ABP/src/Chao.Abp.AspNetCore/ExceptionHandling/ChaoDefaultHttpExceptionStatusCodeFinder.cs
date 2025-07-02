using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Authorization;

namespace Chao.Abp.AspNetCore.ExceptionHandling;

public class ChaoDefaultHttpExceptionStatusCodeFinder(IOptions<AbpExceptionHttpStatusCodeOptions> options) : DefaultHttpExceptionStatusCodeFinder(options)
{
    public virtual ChaoAbpExceptionHttpStatusCodeOption ChaoAbpExceptionHttpStatusCodeOption => ChaoAbpExceptionHttpStatusCodeOptions!.Value;
    public virtual IOptions<ChaoAbpExceptionHttpStatusCodeOption>? ChaoAbpExceptionHttpStatusCodeOptions { get; set; }

    public override HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception)
    {
        if (exception is AbpAuthorizationException)
        {
            return HttpStatusCode.MethodNotAllowed;
        }
        if (exception is IBusinessException)
        {
            if (ChaoAbpExceptionHttpStatusCodeOption.BusinessExceptionHttpStatusCode.HasValue == true)
            {
                return ChaoAbpExceptionHttpStatusCodeOption.BusinessExceptionHttpStatusCode.Value;
            }
        }
        return base.GetStatusCode(httpContext, exception);
    }
}