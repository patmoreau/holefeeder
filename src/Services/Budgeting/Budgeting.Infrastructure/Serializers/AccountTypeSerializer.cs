using System;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MongoDB.Bson.Serialization;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers
{
    public class AccountTypeSerializer : IBsonSerializer<AccountType>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, AccountType value)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            context.Writer.WriteString(value?.Name);
        }

        public AccountType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var type = context.Reader.ReadString();
            return Enumeration.FromName<AccountType>(type);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, value as AccountType);
        }

        public Type ValueType => typeof(AccountType);
    }
}
