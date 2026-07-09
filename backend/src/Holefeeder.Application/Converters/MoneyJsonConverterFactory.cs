using System.Text.Json;
using System.Text.Json.Serialization;

using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Converters;

public class MoneyJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(Money);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        new MoneyJsonConverter();
}
