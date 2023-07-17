using Chao.Abp.Timing;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Chao.Abp.Json.SystemTextJson.JsonConverters;

public class ChaoNullableDateTimeOffsetConverter : JsonConverter<DateTimeOffset?>, ITransientDependency
{
    private readonly IChaoClock _clock;
    private readonly AbpJsonOptions _options;

    public ChaoNullableDateTimeOffsetConverter(IChaoClock clock, IOptions<AbpJsonOptions> abpJsonOptions)
    {
        _clock = clock;
        _options = abpJsonOptions.Value;
    }

    public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TryGetDateTimeOffset(out var d2))
        {
            return d2;
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (DateTimeOffset.TryParse(s, out var d1))
            {
                return d1;
            }
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
    {
        if (value.HasValue == false)
        {
            writer.WriteNullValue();
        }
        else
        {
            var ticks = value.Value.ToUniversalTime().Ticks - _clock.Genesis.Ticks;
            ticks = (long)(TimeSpan.FromTicks(ticks).TotalMilliseconds);
            writer.WriteNumberValue(ticks);
        }
    }
}