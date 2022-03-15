using System;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions;

public class ObjectStoreDomainException : Exception
{
    public ObjectStoreDomainException(string context, string message) : base(message)
    {
        Context = context;
    }

    public string Context { get; }
}
