using System;
using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo.Exceptions
{
    public class QueryException : Exception
    {
        public QueryException(string propertyName) : base($"Invalid property {propertyName}") { }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private QueryException() { }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private QueryException(string message, Exception innerException) : base(message, innerException) { }
    }
}
