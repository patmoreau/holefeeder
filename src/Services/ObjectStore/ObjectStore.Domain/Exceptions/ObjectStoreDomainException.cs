using System;

namespace DrifterApps.Holefeeder.ObjectStore.Domain.Exceptions;

public class ObjectStoreDomainException : Exception
{
    public string Context { get; }

    public ObjectStoreDomainException(string context, string message) : base(message)
    {
        Context = context;
    }
}
