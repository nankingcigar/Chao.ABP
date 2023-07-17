/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 08:36:16
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 08:53:16
 */

using Chao.Abp.ResultHandling.Dto;
using Chao.Abp.ResultHandling.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.ApiExploring;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Reflection;

namespace Chao.Abp.AspNetCore.Mvc.ApiExploring;

public class ChaoAbpRemoteServiceApiDescriptionProvider : IApiDescriptionProvider, ITransientDependency
{
    private readonly IModelMetadataProvider _modelMetadataProvider;
    private readonly MvcOptions _mvcOptions;
    private readonly AbpRemoteServiceApiDescriptionProviderOptions _options;

    public ChaoAbpRemoteServiceApiDescriptionProvider(
        IModelMetadataProvider modelMetadataProvider,
        IOptions<MvcOptions> mvcOptionsAccessor,
        IOptions<AbpRemoteServiceApiDescriptionProviderOptions> optionsAccessor)
    {
        _modelMetadataProvider = modelMetadataProvider;
        _mvcOptions = mvcOptionsAccessor.Value;
        _options = optionsAccessor.Value;
    }

    public int Order => -999;

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        foreach (var apiResponseType in GetApiResponseTypes())
        {
            foreach (var result in context.Results.Where(x => x.IsRemoteService() == true))
            {
                var actionProducesResponseTypeAttributes =
                    ReflectionHelper.GetAttributesOfMemberOrDeclaringType<ProducesResponseTypeAttribute>(
                        result.ActionDescriptor.GetMethodInfo());
                if (actionProducesResponseTypeAttributes.Any(x => x.StatusCode == apiResponseType.StatusCode))
                {
                    continue;
                }

                result.SupportedResponseTypes.AddIfNotContains(x => x.StatusCode == apiResponseType.StatusCode,
                    () => apiResponseType);
            }
        }
        Dictionary<Type, Type> typeDict = new Dictionary<Type, Type>();
        foreach (var result in context.Results.Where(x => x.IsRemoteService() == true))
        {
            var attribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<WrapResultAttribute>(result.ActionDescriptor.GetMethodInfo());
            foreach (var supportedResponseType in result.SupportedResponseTypes)
            {
                if (supportedResponseType.StatusCode == 200)
                {
                    if (attribute != null && attribute.WrapOnSuccess == false)
                    {
                        continue;
                    }
                    if (supportedResponseType.Type != typeof(void))
                    {
                        if (typeDict.ContainsKey(supportedResponseType.Type))
                        {
                            supportedResponseType.Type = typeDict[supportedResponseType.Type];
                        }
                        else
                        {
                            var t = typeof(ApiResponse<>).MakeGenericType(supportedResponseType.Type);
                            typeDict.Add(supportedResponseType.Type, t);
                            supportedResponseType.Type = typeDict[supportedResponseType.Type];
                        }
                    }
                    else
                    {
                        supportedResponseType.Type = typeof(ApiResponse);
                    }
                }
                else
                {
                    if (attribute != null && attribute.WrapOnError == false)
                    {
                        continue;
                    }
                    supportedResponseType.Type = typeof(ApiResponseError);
                }
                supportedResponseType.ModelMetadata = _modelMetadataProvider.GetMetadataForType(supportedResponseType.Type);
            }
        }
    }

    protected virtual IEnumerable<ApiResponseType> GetApiResponseTypes()
    {
        foreach (var apiResponse in _options.SupportedResponseTypes)
        {
            apiResponse.ModelMetadata = _modelMetadataProvider.GetMetadataForType(apiResponse.Type);

            foreach (var responseTypeMetadataProvider in _mvcOptions.OutputFormatters.OfType<IApiResponseTypeMetadataProvider>())
            {
                var formatterSupportedContentTypes = responseTypeMetadataProvider.GetSupportedContentTypes(null, apiResponse.Type);
                if (formatterSupportedContentTypes == null)
                {
                    continue;
                }

                foreach (var formatterSupportedContentType in formatterSupportedContentTypes)
                {
                    apiResponse.ApiResponseFormats.Add(new ApiResponseFormat
                    {
                        Formatter = (IOutputFormatter)responseTypeMetadataProvider,
                        MediaType = formatterSupportedContentType
                    });
                }
            }
        }
        return _options.SupportedResponseTypes;
    }
}