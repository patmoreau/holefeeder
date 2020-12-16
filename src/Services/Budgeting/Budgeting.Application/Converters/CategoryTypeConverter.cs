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
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return type;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string propertyName = reader.GetString();
                    if (propertyName.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }
                        var typeDiscriminator = reader.GetInt32();
                        type = typeDiscriminator switch
                        {
                            1 => CategoryType.Expense,
                            2 => CategoryType.Gain,
                            _ => throw new JsonException()
                        };
                    }
                }
            }

            throw new JsonException();
            // TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
            // Person person = typeDiscriminator switch
            // {
            //     TypeDiscriminator.Customer => new Customer(),
            //     TypeDiscriminator.Employee => new Employee(),
            //     _ => throw new JsonException()
            // };
        }

        public override void Write(Utf8JsonWriter writer, CategoryType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
