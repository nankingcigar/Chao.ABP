using Chao.Abp.Timing;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson.JsonConverters;

namespace Chao.Abp.Json.SystemTextJson.JsonConverters;

public class ChaoAbpDateTimeConverter : AbpDateTimeConverter
{
    private readonly IChaoClock _clock;
    private readonly AbpJsonOptions _options;

    public ChaoAbpDateTimeConverter(IChaoClock clock, IOptions<AbpJsonOptions> abpJsonOptions) : base(clock, abpJsonOptions)
    {
        _clock = clock;
        _options = abpJsonOptions.Value;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return _clock.Genesis.AddMilliseconds(reader.GetInt64()).ToLocalTime();
        }
        if (_options.InputDateTimeFormats.Any())
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                foreach (var format in _options.InputDateTimeFormats)
                {
                    if (DateTime.TryParseExact(s, format, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var d1))
                    {
                        return _clock.Normalize(d1);
                    }
                }
            }
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            var s = reader.GetString();
            if (DateTime.TryParse(s, CultureInfo.CurrentUICulture, DateTimeStyles.None, out var d3))
            {
                return d3;
            }
        }
        if (reader.TryGetDateTime(out var d2))
        {
            return _clock.Normalize(d2);
        }
        throw new JsonException("Can't get datetime from the reader!");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var ticks = value.ToUniversalTime().Ticks - _clock.Genesis.Ticks;
        ticks = (long)(TimeSpan.FromTicks(ticks).TotalMilliseconds);
        writer.WriteNumberValue(ticks);
    }
}