using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Converters
{
    public class EnumerationJsonConverter<T> : JsonConverter<T> where T : Enumeration
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsSubclassOf(typeof(Enumeration));
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Invalid Enumeration type {typeToConvert.FullName}");
            }

            var typeName = reader.GetString();
            var type = Enumeration.FromName<T>(typeName);
            if (type is null)
            {
                throw new JsonException($"Invalid {typeof(T).Name}");
            }

            return type;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is not null)
            {
                writer.WriteStringValue(value.Name);
            }
        }
    }
}
