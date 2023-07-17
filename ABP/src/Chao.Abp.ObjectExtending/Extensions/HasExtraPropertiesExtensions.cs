using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using Volo.Abp.Reflection;

namespace Volo.Abp.Data;

public static class HasExtraPropertiesExtensions
{
    public static TProperty GetPropertyEnhancement<TProperty>(this IHasExtraProperties source, string name, TProperty defaultValue = default)
    {
        var value = source.GetProperty(name);
        if (value == null)
        {
            return defaultValue;
        }

        var conversionType = typeof(TProperty);
        if (TypeHelper.IsPrimitiveExtended(typeof(TProperty), includeEnums: true) == true)
        {
            if (TypeHelper.IsNullable(conversionType) == true)
            {
                conversionType = conversionType.GetFirstGenericArgumentIfNullable();
            }

            if (conversionType == typeof(Guid))
            {
                return (TProperty)TypeDescriptor.GetConverter(conversionType).ConvertFromInvariantString(value.ToString());
            }
            if (conversionType.IsEnum == true)
            {
                return (TProperty)Enum.ToObject(conversionType, value);
            }
            else
            {
                return (TProperty)Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
            }
        }
        else if (conversionType.IsClass == true)
        {
            return JsonSerializer.Deserialize<TProperty>(value.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
        else if (conversionType.IsGenericType == true)
        {
            return JsonSerializer.Deserialize<TProperty>(value.ToString(), new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }

        throw new AbpException("GetProperty<TProperty> does not support non-primitive or non-class types. Use non-generic GetProperty method and handle type casting manually.");
    }

    public static void MapExtraProperties<TSource, TDestination>(
                [NotNull] this TSource source,
        [NotNull] TDestination destination)
        where TSource : IHasExtraProperties
        where TDestination : IHasExtraProperties
    {
        foreach (var key in source.ExtraProperties.Keys)
        {
            destination.SetProperty(key, source.GetProperty(key));
        }
    }
}