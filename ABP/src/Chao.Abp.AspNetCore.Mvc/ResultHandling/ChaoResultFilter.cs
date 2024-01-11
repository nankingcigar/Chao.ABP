using Chao.Abp.ResultHandling.Dto;
using Chao.Abp.ResultHandling.Model;
using Chao.Abp.ResultHandling.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Reflection;

namespace Chao.Abp.AspNetCore.Mvc.ResultHandler;

public class ChaoResultFilter(IOptions<ChaoAbpResultHandlingOption> chaoAbpResultHandlingOptionOptions) : IAsyncActionFilter, ITransientDependency
{
    private readonly ChaoAbpResultHandlingOption _chaoAbpResultHandlingOption = chaoAbpResultHandlingOptionOptions.Value;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();
        if (!ShouldHandleResult(executedContext))
        {
            return;
        }
        if (executedContext.Result is ObjectResult result && !result.StatusCode.HasValue)
        {
            result.Value = new ApiResponse(result.Value);
            result.DeclaredType = typeof(ApiResponse);
        }
    }

    protected virtual bool ShouldHandleResult(ActionExecutedContext context)
    {
        if (context.Exception != null)
        {
            return false;
        }
        var attribute = ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<WrapResultAttribute>(context.ActionDescriptor.GetMethodInfo());
        if (attribute != null && attribute.WrapOnSuccess == false)
        {
            return false;
        }
        var controllerType = context.Controller.GetType();
        if (_chaoAbpResultHandlingOption.ExcludeControllerTypes.Any(c => c == controllerType) == true)
        {
            return false;
        }
        if (context.ActionDescriptor.IsControllerAction() &&
            context.ActionDescriptor.HasObjectResult())
        {
            return true;
        }
        return false;
    }
}