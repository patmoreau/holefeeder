using System.Text.Json;
using System.Text.Json.Serialization;

using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Converters;

public class CategoryColorJsonConverter : JsonConverter<CategoryColor>
{
    public override CategoryColor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        var result = CategoryColor.Create(value ?? CategoryColor.Empty);
        if (result.IsFailure)
        {
            throw new JsonException(result.Error.Description);
        }
        return result.Value;
    }

    public override void Write(Utf8JsonWriter writer, CategoryColor value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteStringValue(value.ToString());
    }
}
