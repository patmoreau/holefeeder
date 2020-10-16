using System;
using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Exceptions
{
    public class HolefeederInfrastructureException : Exception
    {
        public HolefeederInfrastructureException(string propertyName) : base($"Invalid property {propertyName}") { }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private HolefeederInfrastructureException() { }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private HolefeederInfrastructureException(string message, Exception innerException) : base(message, innerException) { }
    }
}
