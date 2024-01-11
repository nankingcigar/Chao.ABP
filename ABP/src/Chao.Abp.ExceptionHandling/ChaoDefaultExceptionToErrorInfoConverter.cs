using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.ExceptionHandling.Localization;
using Volo.Abp.Http;
using Volo.Abp.Localization.ExceptionHandling;

namespace Chao.Abp.ExceptionHandling;

public class ChaoDefaultExceptionToErrorInfoConverter(
    IOptions<AbpExceptionLocalizationOptions> localizationOptions,
    IStringLocalizerFactory stringLocalizerFactory,
    IStringLocalizer<AbpExceptionHandlingResource> stringLocalizer,
    IServiceProvider serviceProvider) : DefaultExceptionToErrorInfoConverter(localizationOptions, stringLocalizerFactory, stringLocalizer, serviceProvider)
{
    protected override RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception, AbpExceptionHandlingOptions options)
    {
        var errorInfo = base.CreateErrorInfoWithoutCode(exception, options);
        exception = TryToGetActualException(exception);
        if (exception is BusinessException && exception is IHasErrorCode hasErrorCodeException)
        {
            errorInfo.Code = hasErrorCodeException.Code;
        }
        return errorInfo;
    }
}