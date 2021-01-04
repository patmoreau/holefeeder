using System;
using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Holefeeder.Framework.Mongo.SeedWork
{
    public class MongoInvalidPropertyNameException : Exception
    {
        public MongoInvalidPropertyNameException(string propertyName) : base($"Invalid property {propertyName}") { }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private MongoInvalidPropertyNameException() { }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private MongoInvalidPropertyNameException(string message, Exception innerException) : base(message, innerException) { }
    }
}
