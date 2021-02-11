using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Application.Converters
{
    public class AccountTypeConverter : JsonConverter<AccountType>
    {
        public override AccountType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            AccountType type = AccountType.Checking;
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
                            try
                            {
                                type = Enumeration.FromValue<AccountType>(typeId);
                            }
                            catch (InvalidOperationException e)
                            {
                                throw new JsonException(e.Message, e);
                            }
                        }

                        break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, AccountType value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
