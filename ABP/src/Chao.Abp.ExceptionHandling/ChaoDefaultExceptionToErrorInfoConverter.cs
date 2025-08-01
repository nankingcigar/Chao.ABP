﻿using Chao.Abp.ExceptionHandling.Option;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.ExceptionHandling.Localization;
using Volo.Abp.Http;
using Volo.Abp.Http.Client;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation;

namespace Chao.Abp.ExceptionHandling;

public class ChaoDefaultExceptionToErrorInfoConverter(
    IOptions<AbpExceptionLocalizationOptions> localizationOptions,
    IStringLocalizerFactory stringLocalizerFactory,
    IStringLocalizer<AbpExceptionHandlingResource> stringLocalizer,
    IServiceProvider serviceProvider,
    IOptions<ChaoAbpExceptionHandlingOption> chaoAbpExceptionHandlingOptions
    ) : IExceptionToErrorInfoConverter, ITransientDependency
{
    public virtual ChaoAbpExceptionHandlingOption ChaoAbpExceptionHandlingOption => chaoAbpExceptionHandlingOptions.Value;
    public virtual IStringLocalizer<AbpExceptionHandlingResource> L => stringLocalizer;
    public virtual AbpExceptionLocalizationOptions LocalizationOptions => localizationOptions.Value;

    public virtual RemoteServiceErrorInfo Convert(Exception exception, bool includeSensitiveDetails)
    {
        var exceptionHandlingOptions = CreateDefaultOptions();
        exceptionHandlingOptions.SendExceptionsDetailsToClients = includeSensitiveDetails;
        exceptionHandlingOptions.SendStackTraceToClients = includeSensitiveDetails;

        var errorInfo = CreateErrorInfoWithoutCode(exception, exceptionHandlingOptions);

        TryToSetErrorCode(exception, errorInfo);

        return errorInfo;
    }

    public virtual RemoteServiceErrorInfo Convert(Exception exception, Action<AbpExceptionHandlingOptions>? options = null)
    {
        var exceptionHandlingOptions = CreateDefaultOptions();
        options?.Invoke(exceptionHandlingOptions);

        var errorInfo = CreateErrorInfoWithoutCode(exception, exceptionHandlingOptions);

        TryToSetErrorCode(exception, errorInfo);

        return errorInfo;
    }

    public virtual MethodInfo? GetRealMethodFromAsyncStateMachine(MethodBase? method)
    {
        if (method == null) return null;
        var declaringType = method.DeclaringType;
        var outerType = declaringType?.DeclaringType;
        if (outerType == null) return method as MethodInfo;
        var generatedTypeName = declaringType!.Name;
        if (generatedTypeName.Contains("<") && generatedTypeName.Contains(">"))
        {
            var methodName = generatedTypeName.Split('<', '>')[1];
            var realMethod = outerType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == methodName);
            return realMethod ?? method as MethodInfo;
        }
        return method as MethodInfo;
    }

    public virtual void TryToSetErrorCode(Exception exception, RemoteServiceErrorInfo remoteServiceErrorInfo)
    {
        exception = TryToGetActualException(exception);
        if (exception is IHasErrorCode hasErrorCode)
        {
            var errorCodes = new List<string?>();
            var stackTrace = new StackTrace(exception, 0, false);
            var frames = stackTrace.GetFrames()!;
            foreach (var frame in frames)
            {
                var method = GetRealMethodFromAsyncStateMachine(frame.GetMethod());
                if (method == null) continue;
                var methodAttr = method.GetCustomAttribute<ErrorCodeAttribute>();
                var classAttr = method.DeclaringType?.GetCustomAttribute<ErrorCodeAttribute>();
                if (methodAttr != null || classAttr != null)
                {
                    if (classAttr != null)
                    {
                        errorCodes.AddRange(classAttr.Prefix);
                    }
                    else if (methodAttr?.ClassName == true)
                    {
                        errorCodes.Add(method.Name);
                    }
                    if (methodAttr != null)
                    {
                        errorCodes.AddRange(methodAttr.Prefix);
                    }
                    else if (classAttr?.MethodName == true)
                    {
                        errorCodes.Add(method?.Name);
                    }
                    break;
                }
            }
            errorCodes.Add(hasErrorCode.Code);
            var code = errorCodes.Count > 1 ? string.Join(ChaoAbpExceptionHandlingOption.ExceptionCodeJoinString, errorCodes) : errorCodes[0];
            remoteServiceErrorInfo.Code = code;
        }
    }

    protected virtual void AddExceptionToDetails(Exception exception, StringBuilder detailBuilder, bool sendStackTraceToClients)
    {
        //Exception Message
        detailBuilder.AppendLine(exception.GetType().Name + ": " + exception.Message);

        //Additional info for UserFriendlyException
        if (exception is IUserFriendlyException &&
            exception is IHasErrorDetails)
        {
            var details = ((IHasErrorDetails)exception).Details;
            if (!details.IsNullOrEmpty())
            {
                detailBuilder.AppendLine(details);
            }
        }

        //Additional info for AbpValidationException
        if (exception is AbpValidationException validationException)
        {
            if (validationException.ValidationErrors.Count > 0)
            {
                detailBuilder.AppendLine(GetValidationErrorNarrative(validationException));
            }
        }

        //Exception StackTrace
        if (sendStackTraceToClients && !string.IsNullOrEmpty(exception.StackTrace))
        {
            detailBuilder.AppendLine("STACK TRACE: " + exception.StackTrace);
        }

        //Inner exception
        if (exception.InnerException != null)
        {
            AddExceptionToDetails(exception.InnerException, detailBuilder, sendStackTraceToClients);
        }

        //Inner exceptions for AggregateException
        if (exception is AggregateException aggException)
        {
            if (aggException.InnerExceptions.IsNullOrEmpty())
            {
                return;
            }

            foreach (var innerException in aggException.InnerExceptions)
            {
                AddExceptionToDetails(innerException, detailBuilder, sendStackTraceToClients);
            }
        }
    }

    protected virtual AbpExceptionHandlingOptions CreateDefaultOptions()
    {
        return new AbpExceptionHandlingOptions
        {
            SendExceptionsDetailsToClients = false,
            SendStackTraceToClients = true
        };
    }

    protected virtual RemoteServiceErrorInfo CreateDetailedErrorInfoFromException(Exception exception, bool sendStackTraceToClients)
    {
        var detailBuilder = new StringBuilder();

        AddExceptionToDetails(exception, detailBuilder, sendStackTraceToClients);

        var errorInfo = new RemoteServiceErrorInfo(exception.Message, detailBuilder.ToString(), data: exception.Data);

        if (exception is AbpValidationException)
        {
            errorInfo.ValidationErrors = GetValidationErrorInfos((exception as AbpValidationException)!);
        }

        return errorInfo;
    }

    protected virtual RemoteServiceErrorInfo CreateEntityNotFoundError(EntityNotFoundException exception)
    {
        if (exception.EntityType != null)
        {
            return new RemoteServiceErrorInfo(
                string.Format(
                    L["EntityNotFoundErrorMessage"],
                    exception.EntityType.Name,
                    exception.Id
                )
            );
        }

        return new RemoteServiceErrorInfo(exception.Message);
    }

    protected virtual RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception, AbpExceptionHandlingOptions options)
    {
        if (options.SendExceptionsDetailsToClients)
        {
            return CreateDetailedErrorInfoFromException(exception, options.SendStackTraceToClients);
        }

        exception = TryToGetActualException(exception);

        if (exception is AbpRemoteCallException remoteCallException && remoteCallException.Error != null)
        {
            var remoteServiceErrorInfo = remoteCallException.Error;
            if (remoteServiceErrorInfo.Message == AbpExceptionHandlingConsts.Unauthorized)
            {
                remoteServiceErrorInfo.Message = L[AbpExceptionHandlingConsts.Unauthorized];
            }
            if (remoteServiceErrorInfo.Details == AbpExceptionHandlingConsts.SessionExpired)
            {
                remoteServiceErrorInfo.Details = L[AbpExceptionHandlingConsts.SessionExpired];
            }
            return remoteServiceErrorInfo;
        }

        if (exception is AbpDbConcurrencyException)
        {
            return new RemoteServiceErrorInfo(L["AbpDbConcurrencyErrorMessage"]);
        }

        if (exception is EntityNotFoundException)
        {
            return CreateEntityNotFoundError((exception as EntityNotFoundException)!);
        }

        var errorInfo = new RemoteServiceErrorInfo();

        if (exception is IUserFriendlyException || exception is AbpRemoteCallException)
        {
            errorInfo.Message = exception.Message;
            errorInfo.Details = (exception as IHasErrorDetails)?.Details;
        }

        if (exception is IHasValidationErrors)
        {
            if (errorInfo.Message.IsNullOrEmpty())
            {
                errorInfo.Message = L["ValidationErrorMessage"];
            }

            if (errorInfo.Details.IsNullOrEmpty())
            {
                errorInfo.Details = GetValidationErrorNarrative((exception as IHasValidationErrors)!);
            }

            errorInfo.ValidationErrors = GetValidationErrorInfos((exception as IHasValidationErrors)!);
        }

        TryToLocalizeExceptionMessage(exception, errorInfo);

        if (errorInfo.Message.IsNullOrEmpty())
        {
            errorInfo.Message = L["InternalServerErrorMessage"];
        }

        if (options.SendExceptionDataToClientTypes.Any(t => t.IsInstanceOfType(exception)))
        {
            errorInfo.Data = exception.Data;
        }

        return errorInfo;
    }

    protected virtual RemoteServiceValidationErrorInfo[] GetValidationErrorInfos(IHasValidationErrors validationException)
    {
        var validationErrorInfos = new List<RemoteServiceValidationErrorInfo>();

        foreach (var validationResult in validationException.ValidationErrors)
        {
            var validationError = new RemoteServiceValidationErrorInfo(validationResult.ErrorMessage!);

            if (validationResult.MemberNames != null && validationResult.MemberNames.Any())
            {
                validationError.Members = validationResult.MemberNames.Select(m => m.ToCamelCase()).ToArray();
            }

            validationErrorInfos.Add(validationError);
        }

        return validationErrorInfos.ToArray();
    }

    protected virtual string GetValidationErrorNarrative(IHasValidationErrors validationException)
    {
        var detailBuilder = new StringBuilder();
        detailBuilder.AppendLine(L["ValidationNarrativeErrorMessageTitle"]);

        foreach (var validationResult in validationException.ValidationErrors)
        {
            detailBuilder.AppendFormat(" - {0}", validationResult.ErrorMessage);
            detailBuilder.AppendLine();
        }

        return detailBuilder.ToString();
    }

    protected virtual Exception TryToGetActualException(Exception exception)
    {
        if (exception is AggregateException aggException && aggException.InnerException != null)
        {
            if (aggException.InnerException is AbpValidationException ||
                aggException.InnerException is AbpAuthorizationException ||
                aggException.InnerException is EntityNotFoundException ||
                aggException.InnerException is IBusinessException)
            {
                return aggException.InnerException;
            }
        }

        return exception;
    }

    protected virtual void TryToLocalizeExceptionMessage(Exception exception, RemoteServiceErrorInfo errorInfo)
    {
        if (exception is ILocalizeErrorMessage localizeErrorMessageException)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                errorInfo.Message = localizeErrorMessageException.LocalizeMessage(new LocalizationContext(scope.ServiceProvider));
            }

            return;
        }

        if (!(exception is IHasErrorCode exceptionWithErrorCode))
        {
            return;
        }

        if (exceptionWithErrorCode.Code.IsNullOrWhiteSpace() ||
            !exceptionWithErrorCode.Code!.Contains(":"))
        {
            return;
        }

        var codeNamespace = exceptionWithErrorCode.Code.Split(':')[0];

        var localizationResourceType = LocalizationOptions.ErrorCodeNamespaceMappings.GetOrDefault(codeNamespace);
        if (localizationResourceType == null)
        {
            return;
        }

        var stringLocalizer = stringLocalizerFactory.Create(localizationResourceType);
        var localizedString = stringLocalizer[exceptionWithErrorCode.Code];
        if (localizedString.ResourceNotFound)
        {
            return;
        }

        var localizedValue = localizedString.Value;

        if (exception.Data != null && exception.Data.Count > 0)
        {
            foreach (var key in exception.Data.Keys)
            {
                localizedValue = localizedValue.Replace("{" + key + "}", exception.Data[key]?.ToString());
            }
        }

        errorInfo.Message = localizedValue;
    }
}