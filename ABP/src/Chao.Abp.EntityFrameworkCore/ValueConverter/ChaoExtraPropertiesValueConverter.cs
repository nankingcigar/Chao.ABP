using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Volo.Abp.Data;
using Volo.Abp.Json.SystemTextJson.JsonConverters;
using Volo.Abp.ObjectExtending;

namespace Chao.Abp.EntityFrameworkCore.ValueConverter;

public class ChaoExtraPropertiesValueConverter : ValueConverter<ExtraPropertyDictionary, string>
{
    public static readonly JsonSerializerOptions DeserializeOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        Converters ={
            new ObjectToInferredTypesConverter()
        }
    };

    public static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public ChaoExtraPropertiesValueConverter(Type entityType)
            : base(
        d => SerializeObject(d, entityType),
        s => DeserializeObject(s, entityType))
    {
    }

    private static ExtraPropertyDictionary DeserializeObject(string extraPropertiesAsJson, Type? entityType)
    {
        if (extraPropertiesAsJson.IsNullOrEmpty() || extraPropertiesAsJson == "{}")
        {
            return new ExtraPropertyDictionary();
        }

        var dictionary = JsonSerializer.Deserialize<ExtraPropertyDictionary>(extraPropertiesAsJson, DeserializeOptions) ??
                            new ExtraPropertyDictionary();

        if (entityType != null)
        {
            var objectExtension = ObjectExtensionManager.Instance.GetOrNull(entityType);
            if (objectExtension != null)
            {
                foreach (var property in objectExtension.GetProperties())
                {
                    dictionary[property.Name] = GetNormalizedValue(dictionary!, property);
                }
            }
        }

        return dictionary;
    }

    private static object? GetNormalizedValue(
    Dictionary<string, object> dictionary,
    ObjectExtensionPropertyInfo property)
    {
        var value = dictionary.GetOrDefault(property.Name);
        if (value == null)
        {
            return property.GetDefaultValue();
        }

        try
        {
            if (property.Type.IsEnum)
            {
                return Enum.Parse(property.Type, value.ToString()!, true);
            }

            //return Convert.ChangeType(value, property.Type);
            return value;
        }
        catch
        {
            return value;
        }
    }

    private static string SerializeObject(ExtraPropertyDictionary extraProperties, Type? entityType)
    {
        var copyDictionary = new Dictionary<string, object?>(extraProperties);

        if (entityType != null)
        {
            var objectExtension = ObjectExtensionManager.Instance.GetOrNull(entityType);
            if (objectExtension != null)
            {
                foreach (var property in objectExtension.GetProperties())
                {
                    if (property.IsMappedToFieldForEfCore())
                    {
                        copyDictionary.Remove(property.Name);
                    }
                }
            }
        }

        return JsonSerializer.Serialize(copyDictionary, SerializeOptions);
    }
}