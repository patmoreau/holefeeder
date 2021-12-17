using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;

public class NotFoundDomainException : Exception
{
    public string Context { get; }

    private NotFoundDomainException(string context, string message) : base(message)
    {
        Context = context;
    }

    public static NotFoundDomainException Create<T>() where T : IAggregateRoot
        => new NotFoundDomainException(typeof(T).Name, $"{typeof(T).Name} not found.");
}
