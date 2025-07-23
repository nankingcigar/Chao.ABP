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

public class ChaoAbpExceptionFilter(IOptions<ChaoAbpResultHandlingOption> chaoAbpResultHandlingOptionOptions) : AbpExceptionFilter()
{
    public virtual ChaoAbpResultHandlingOption ChaoAbpResultHandlingOption => chaoAbpResultHandlingOptionOptions.Value;

    protected override async Task HandleAndWrapException(ExceptionContext context)
    {
        await base.HandleAndWrapException(context);
        if (ShouldHandleException(context))
        {
            if (context.Result == null)
            {
                return;
            }
            var result = (context.Result as ObjectResult)!;
            var error = (result.Value as RemoteServiceErrorResponse)!.Error;
            result.Value = new ApiResponseError(error);
        }
    }

    protected override bool ShouldHandleException(ExceptionContext context)
    {
        var attribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<WrapResultAttribute>(context.ActionDescriptor.GetMethodInfo(), WrapResultAttribute.Default);
        if (attribute != null && attribute.WrapOnError == false)
        {
            return false;
        }
        var controllerType = context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType();
        if (ChaoAbpResultHandlingOption.ExcludeControllerTypes.Any(c => c == controllerType) == true)
        {
            return false;
        }
        if (attribute != null && attribute.WrapOnError == true)
        {
            return true;
        }
        if (base.ShouldHandleException(context) == true)
        {
            return true;
        }
        return false;
    }
}