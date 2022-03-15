using System;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

namespace DrifterApps.Holefeeder.Budgeting.Application.Models;

public record AccountViewModel
{
    public Guid Id { get; init; }

    public AccountType Type { get; init; } = null!;

    public string Name { get; init; } = null!;

    public decimal OpenBalance { get; init; }

    public DateTime OpenDate { get; init; }

    public int TransactionCount { get; init; }

    public decimal Balance { get; init; }

    public DateTime? Updated { get; init; }

    public string Description { get; init; } = null!;

    public bool Favorite { get; init; }

    public bool Inactive { get; init; }
}
