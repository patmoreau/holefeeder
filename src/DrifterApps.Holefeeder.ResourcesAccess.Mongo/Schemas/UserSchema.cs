using System;
using MongoDB.Bson.Serialization.Attributes;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas
{
    [BsonIgnoreExtraElements]
    public class UserSchema : BaseSchema
    {
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [BsonElement("lastName")]
        public string LastName { get; set; }

        [BsonElement("emailAddress")]
        public string EmailAddress { get; set; }

        [BsonElement("googleId")]
        public string GoogleId { get; set; }

        [BsonElement("dateJoined"), BsonDateTimeOptions(DateOnly = true)]
        public DateTime DateJoined { get; set; }
    }
}
