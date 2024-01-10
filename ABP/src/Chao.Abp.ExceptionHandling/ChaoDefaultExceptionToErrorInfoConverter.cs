/*
 * @Author: Chao Yang
 * @Date: 2020-11-24 02:27:54
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-11-24 03:06:54
 */

using Chao.Abp.Core.Exception;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.ExceptionHandling.Localization;
using Volo.Abp.Http;
using Volo.Abp.Localization.ExceptionHandling;

namespace Chao.Abp.ExceptionHandling;

public class ChaoDefaultExceptionToErrorInfoConverter : DefaultExceptionToErrorInfoConverter
{
    public ChaoDefaultExceptionToErrorInfoConverter(
        IOptions<AbpExceptionLocalizationOptions> localizationOptions,
        IStringLocalizerFactory stringLocalizerFactory,
        IStringLocalizer<AbpExceptionHandlingResource> stringLocalizer,
        IServiceProvider serviceProvider)
        : base(localizationOptions, stringLocalizerFactory, stringLocalizer, serviceProvider)
    {
    }

    protected override RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception, AbpExceptionHandlingOptions options)
    {
        var errorInfo = base.CreateErrorInfoWithoutCode(exception, options);
        exception = TryToGetActualException(exception);
        if (exception is CodeException && exception is IHasErrorCode hasErrorCodeException)
        {
            errorInfo.Code = hasErrorCodeException.Code;
        }
        return errorInfo;
    }
}