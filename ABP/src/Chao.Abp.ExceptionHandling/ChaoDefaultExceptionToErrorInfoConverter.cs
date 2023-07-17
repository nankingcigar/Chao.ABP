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
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.ExceptionHandling.Localization;
using Volo.Abp.Http;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;

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
        if (exception is IHasErrorCode hasErrorCodeException)
        {
            errorInfo.Code = hasErrorCodeException.Code;
        }
        return errorInfo;
    }

    protected override Exception TryToGetActualException(Exception exception)
    {
        if (exception is AggregateException && exception.InnerException != null)
        {
            var aggException = exception as AggregateException;
            if (aggException.InnerException is AbpValidationException ||
                aggException.InnerException is AbpAuthorizationException ||
                aggException.InnerException is EntityNotFoundException ||
                aggException.InnerException is IBusinessException ||
                aggException.InnerException is ICodeException)
            {
                return aggException.InnerException;
            }
        }
        return exception;
    }
}