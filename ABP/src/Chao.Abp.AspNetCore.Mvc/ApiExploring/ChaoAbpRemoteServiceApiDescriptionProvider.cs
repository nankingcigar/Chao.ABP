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

public class ChaoAbpRemoteServiceApiDescriptionProvider(
    IModelMetadataProvider modelMetadataProvider,
    IOptions<MvcOptions> mvcOptionsAccessor,
    IOptions<AbpRemoteServiceApiDescriptionProviderOptions> optionsAccessor) : IApiDescriptionProvider, ITransientDependency
{
    private readonly IModelMetadataProvider _modelMetadataProvider = modelMetadataProvider;
    private readonly MvcOptions _mvcOptions = mvcOptionsAccessor.Value;
    private readonly AbpRemoteServiceApiDescriptionProviderOptions _options = optionsAccessor.Value;

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
        Dictionary<Type, Type> typeDict = [];
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
                        var type = supportedResponseType.Type!;
                        if (typeDict.TryGetValue(type, out Type? value))
                        {
                            supportedResponseType.Type = value;
                        }
                        else
                        {
                            var t = typeof(ApiResponse<>).MakeGenericType(type);
                            typeDict.Add(type, t);
                            supportedResponseType.Type = typeDict[type];
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
            apiResponse.ModelMetadata = _modelMetadataProvider.GetMetadataForType(apiResponse.Type!);

            foreach (var responseTypeMetadataProvider in _mvcOptions.OutputFormatters.OfType<IApiResponseTypeMetadataProvider>())
            {
                var formatterSupportedContentTypes = responseTypeMetadataProvider.GetSupportedContentTypes(string.Empty, apiResponse.Type!);
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