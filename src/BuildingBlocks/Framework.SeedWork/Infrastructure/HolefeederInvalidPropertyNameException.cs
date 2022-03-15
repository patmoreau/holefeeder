using System;
using System.Diagnostics.CodeAnalysis;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure;

public class HolefeederInvalidPropertyNameException : Exception
{
    public HolefeederInvalidPropertyNameException(string propertyName) : base($"Invalid property {propertyName}")
    {
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private HolefeederInvalidPropertyNameException() { }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private HolefeederInvalidPropertyNameException(string message, Exception innerException) : base(message,
        innerException)
    {
    }
}
