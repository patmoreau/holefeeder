using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.Converters
{
    public class CategoryTypeConverter : JsonConverter<CategoryType>
    {
        public override CategoryType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            CategoryType type = CategoryType.Expense;
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        return type;
                    case JsonTokenType.PropertyName:
                    {
                        var propertyName = reader.GetString();
                        if (propertyName?.Equals("Id", StringComparison.InvariantCultureIgnoreCase)??false)
                        {
                            reader.Read();
                            if (reader.TokenType != JsonTokenType.Number)
                            {
                                throw new JsonException();
                            }
                            var typeId = reader.GetInt32();
                            type = typeId switch
                            {
                                1 => CategoryType.Expense,
                                2 => CategoryType.Gain,
                                _ => throw new JsonException()
                            };
                        }

                        break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, CategoryType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
