using System;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;

public class HolefeederDomainException : Exception
{
    public string Context { get; }

    private HolefeederDomainException(string context, string message) : base(message)
    {
        Context = context;
    }

    public static HolefeederDomainException Create<T>(string message) where T : IAggregateRoot =>
        new(typeof(T).Name, $"{typeof(T).Name} entity error: {message}");
}
