using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Linq;
using Volo.Abp.Timing;

namespace Chao.Abp.AspNetCore.Mvc.ModelBinding;

public class ChaoAbpDateTimeModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType != typeof(DateTime) &&
            context.Metadata.ModelType != typeof(DateTime?))
        {
            return null;
        }
        if (context.Metadata.ContainerType == null)
        {
            if (context.Metadata is DefaultModelMetadata defaultModelMetadata &&
                defaultModelMetadata.Attributes.Attributes.All(x => x.GetType() != typeof(DisableDateTimeNormalizationAttribute)))
            {
                return new ChaoAbpDateTimeModelBinder(context);
            }
        }
        else
        {
            var dateNormalizationDisabledForClass =
                context.Metadata.ContainerType.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true);
            var dateNormalizationDisabledForProperty = context.Metadata.ContainerType
                .GetProperty(context.Metadata.PropertyName!)
                ?.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true);
            if (!dateNormalizationDisabledForClass &&
                dateNormalizationDisabledForProperty != null &&
                !dateNormalizationDisabledForProperty.Value)
            {
                return new ChaoAbpDateTimeModelBinder(context);
            }
        }
        return null;
    }
}