using System.Text.Json;
using System.Text.Json.Serialization;

using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Converters;

public class MoneyJsonConverter : JsonConverter<Money>
{
    public override Money Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetDecimal();
        var money = Money.Create(value);
        if (money.IsFailure)
        {
            throw new JsonException(money.Error.Description);
        }
        return money.Value;
    }

    public override void Write(Utf8JsonWriter writer, Money value, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(value);
        writer.WriteNumberValue(value.Value);
    }
}
