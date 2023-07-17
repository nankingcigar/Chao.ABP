/*
 * @Author: Chao Yang
 * @Date: 2020-12-12 03:43:42
 * @LastEditor: Chao Yang
 * @LastEditTime: 2020-12-12 07:29:57
 */

using Chao.Abp.Timing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Chao.Abp.AspNetCore.Mvc.ModelBinding;

public class ChaoAbpDateTimeModelBinder : IModelBinder
{
    private readonly IChaoClock _clock;
    private readonly SimpleTypeModelBinder _simpleTypeModelBinder;
    private readonly Type _type;

    public ChaoAbpDateTimeModelBinder(ModelBinderProviderContext context)
    {
        _type = context.Metadata.ModelType;
        _clock = context.Services.GetRequiredService<IChaoClock>();
        _simpleTypeModelBinder = new SimpleTypeModelBinder(context.Metadata.ModelType,
            context.Services.GetRequiredService<ILoggerFactory>());
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        var key = val.FirstValue;
        if (key != null && long.TryParse(key, out long value))
        {
            var dateTime = _clock.Genesis.AddMilliseconds(value).ToLocalTime();
            bindingContext.Result = ModelBindingResult.Success(_clock.Normalize(dateTime));
            return;
        }
        await _simpleTypeModelBinder.BindModelAsync(bindingContext);
        if (!bindingContext.Result.IsModelSet)
        {
            return;
        }
        if (_type == typeof(DateTime))
        {
            var dateTime = (DateTime)bindingContext.Result.Model;
            bindingContext.Result = ModelBindingResult.Success(_clock.Normalize(dateTime));
        }
        else
        {
            var dateTime = (DateTime?)bindingContext.Result.Model;
            if (dateTime != null)
            {
                bindingContext.Result = ModelBindingResult.Success(_clock.Normalize(dateTime.Value));
            }
        }
    }
}