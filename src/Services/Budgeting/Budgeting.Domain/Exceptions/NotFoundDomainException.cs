using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;

public class NotFoundDomainException : Exception
{
    private NotFoundDomainException(string context, string message) : base(message)
    {
        Context = context;
    }

    public string Context { get; }

    public static NotFoundDomainException Create<T>() where T : IAggregateRoot
    {
        return new(typeof(T).Name, $"{typeof(T).Name} not found.");
    }
}
