using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;

public class HolefeederDomainException : Exception
{
    private HolefeederDomainException(string context, string message) : base(message)
    {
        Context = context;
    }

    public string Context { get; }

    public static HolefeederDomainException Create<T>(string message) where T : IAggregateRoot
    {
        return new(typeof(T).Name, $"{typeof(T).Name} entity error: {message}");
    }
}
