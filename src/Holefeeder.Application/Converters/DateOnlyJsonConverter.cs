using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Holefeeder.Application.Converters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } dateString && DateTime.TryParse(dateString, out var dateTime))
        {
            return DateOnly.FromDateTime(dateTime);
        }

        throw new JsonException("Unable to convert to DateOnly");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
