using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Authorization;

namespace Chao.Abp.AspNetCore.ExceptionHandling
{
    public class ChaoDefaultHttpExceptionStatusCodeFinder : DefaultHttpExceptionStatusCodeFinder
    {
        public ChaoDefaultHttpExceptionStatusCodeFinder(IOptions<AbpExceptionHttpStatusCodeOptions> options)
          : base(options)
        {
        }

        public override HttpStatusCode GetStatusCode(HttpContext httpContext, Exception exception)
        {
            if (exception is AbpAuthorizationException)
            {
                return HttpStatusCode.MethodNotAllowed;
            }

            return base.GetStatusCode(httpContext, exception);
        }
    }
}