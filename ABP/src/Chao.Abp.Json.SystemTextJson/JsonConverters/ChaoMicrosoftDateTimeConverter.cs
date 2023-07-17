using Chao.Abp.Timing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Chao.Abp.Json.SystemTextJson.JsonConverters;

public class ChaoMicrosoftDateTimeConverter : JsonConverter<DateTime>, ITransientDependency
{
    private static readonly DateTimeOffset Gensis = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static readonly Regex Regex = new Regex(@"^\\?/Date\((-?\d+)([+-])(\d{2})(\d{2})\)\\?/$", RegexOptions.CultureInvariant | RegexOptions.Compiled);
    private readonly IChaoClock _clock;
    private readonly AbpJsonOptions _options;

    public ChaoMicrosoftDateTimeConverter(IChaoClock clock, IOptions<AbpJsonOptions> abpJsonOptions)
    {
        _clock = clock;
        _options = abpJsonOptions.Value;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Reader's TokenType is not String!");
        }
        string s = reader.GetString()!;
        Match match = Regex.Match(s);

        if (
                !match.Success
                || !long.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long unixTime)
                || !int.TryParse(match.Groups[3].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int hours)
                || !int.TryParse(match.Groups[4].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int minutes))
        {
            throw new JsonException($"'{s}' can't parse to DateTime!");
        }
        int sign = match.Groups[2].Value[0] == '+' ? 1 : -1;
        TimeSpan utcOffset = TimeSpan.FromMinutes((sign * hours * 60) + minutes);
        return Gensis.AddMilliseconds(unixTime).ToOffset(utcOffset).LocalDateTime;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var ticks = value.ToUniversalTime().Ticks - _clock.Genesis.Ticks;
        ticks = (long)(TimeSpan.FromTicks(ticks).TotalMilliseconds);
        writer.WriteNumberValue(ticks);
    }
}