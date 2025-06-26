using Chao.Abp.Json.Abstractions;
using Chao.Abp.Timing;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Volo.Abp.Json;
using Volo.Abp.Json.SystemTextJson.JsonConverters;

namespace Chao.Abp.Json.SystemTextJson.JsonConverters;

public class ChaoAbpNullableDateTimeConverter(IChaoClock clock, IOptions<AbpJsonOptions> abpJsonOptions, IOptions<ChaoAbpJsonOption> chaoAbpJsonOptions) : AbpNullableDateTimeConverter(clock, abpJsonOptions)
{
    private readonly AbpJsonOptions _options = abpJsonOptions.Value;

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return clock.Genesis.AddMilliseconds(reader.GetInt64()).ToLocalTime();
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
                        return clock.Normalize(d1);
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
            return clock.Normalize(d2);
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (chaoAbpJsonOptions.Value.DateTimeNumericFormatEnable == true)
        {
            if (value.HasValue == false)
            {
                writer.WriteNullValue();
            }
            else
            {
                var ticks = value.Value.ToUniversalTime().Ticks - clock.Genesis.Ticks;
                ticks = (long)(TimeSpan.FromTicks(ticks).TotalMilliseconds);
                writer.WriteNumberValue(ticks);
            }
        }
        else
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                if (_options.OutputDateTimeFormat.IsNullOrWhiteSpace())
                {
                    writer.WriteStringValue(clock.Normalize(value.Value));
                }
                else
                {
                    writer.WriteStringValue(clock.Normalize(value.Value).ToString(_options.OutputDateTimeFormat, CultureInfo.CurrentUICulture));
                }
            }
        }
    }
}