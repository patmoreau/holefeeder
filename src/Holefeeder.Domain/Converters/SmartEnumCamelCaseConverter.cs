using System.Text.Json;
using System.Text.Json.Serialization;

using Ardalis.SmartEnum;

namespace Holefeeder.Domain.Converters;

internal class SmartEnumCamelCaseConverter<TEnum, TValue> : JsonConverter<TEnum>
    where TEnum : SmartEnum<TEnum, TValue>
    where TValue : IEquatable<TValue>, IComparable<TValue>
{
    public override TEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return string.IsNullOrEmpty(value) ? null : SmartEnum<TEnum, TValue>.FromName(value, ignoreCase: true);
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        var name = value.Name;
        var camelCase = char.ToLowerInvariant(name[0]) + name[1..];
        writer.WriteStringValue(camelCase);
    }
}
