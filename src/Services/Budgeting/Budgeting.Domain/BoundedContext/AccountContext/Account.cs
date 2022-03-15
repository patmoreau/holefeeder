using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Budgeting.Domain.Exceptions;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

namespace DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;

public record Account : IAggregateRoot
{
    public Account(Guid id, AccountType type, string name, DateTime openDate, Guid userId)
    {
        if (id.Equals(default))
        {
            throw HolefeederDomainException.Create<Account>($"{nameof(Id)} is required");
        }

        Id = id;

        Type = type;

        if (string.IsNullOrWhiteSpace(name) || name.Length > 255)
        {
            throw HolefeederDomainException.Create<Account>($"{nameof(Name)} must be from 1 to 255 characters");
        }

        Name = name;

        if (openDate.Equals(default))
        {
            throw HolefeederDomainException.Create<Account>($"{nameof(OpenDate)} is required");
        }

        OpenDate = openDate;

        if (userId.Equals(default))
        {
            throw HolefeederDomainException.Create<Account>($"{nameof(UserId)} is required");
        }

        UserId = userId;

        Cashflows = ImmutableList<Guid>.Empty;
    }

    public Guid Id { get; }

    public AccountType Type { get; init; }

    public string Name { get; init; }

    public bool Favorite { get; init; }

    public decimal OpenBalance { get; init; }

    public DateTime OpenDate { get; init; }

    public string Description { get; init; } = string.Empty;

    public bool Inactive { get; init; }

    public Guid UserId { get; }

    public IReadOnlyList<Guid> Cashflows { get; init; }

    public static Account Create(AccountType type, string name, decimal openBalance, DateTime openDate,
        string description, Guid userId)
    {
        return new(Guid.NewGuid(), type, name, openDate, userId) {OpenBalance = openBalance, Description = description};
    }

    public Account Close()
    {
        if (Inactive)
        {
            throw HolefeederDomainException.Create<Account>("Account already closed");
        }

        if (Cashflows.Any())
        {
            throw HolefeederDomainException.Create<Account>("Account has active cashflows");
        }

        return this with {Inactive = true};
    }
}
