/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 08:36:16
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 08:53:16
 */

using Chao.Abp.ResultHandling.Dto;
using Chao.Abp.ResultHandling.Model;
using Chao.Abp.ResultHandling.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.Http;
using Volo.Abp.Reflection;

namespace Chao.Abp.AspNetCore.Mvc.ExceptionHandling;

public class ChaoAbpExceptionFilter : AbpExceptionFilter
{
    private ChaoAbpResultHandlingOption _chaoAbpResultHandlingOption;

    public ChaoAbpExceptionFilter(IOptions<ChaoAbpResultHandlingOption> chaoAbpResultHandlingOptionOptions)
    : base()
    {
        _chaoAbpResultHandlingOption = chaoAbpResultHandlingOptionOptions.Value;
    }

    protected override async Task HandleAndWrapException(ExceptionContext context)
    {
        await base.HandleAndWrapException(context);
        if (ShouldHandleException(context))
        {
            if (context.Result == null)
            {
                return;
            }
            var result = context.Result as ObjectResult;
            var error = (result.Value as RemoteServiceErrorResponse).Error;
            result.Value = new ApiResponseError(error);
        }
    }

    protected override bool ShouldHandleException(ExceptionContext context)
    {
        var attribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<WrapResultAttribute>(context.ActionDescriptor.GetMethodInfo());
        if (attribute != null && attribute.WrapOnError == false)
        {
            return false;
        }
        var controllerType = context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType();
        if (_chaoAbpResultHandlingOption.ExcludeControllerTypes.Any(c => c == controllerType) == true)
        {
            return false;
        }
        return base.ShouldHandleException(context);
    }
}