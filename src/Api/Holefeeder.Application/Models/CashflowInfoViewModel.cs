using System.Collections.Immutable;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Enumerations;

namespace Holefeeder.Application.Models;

public record CashflowInfoViewModel
{
    private readonly ImmutableArray<string> _tags = ImmutableArray<string>.Empty;

    public Guid Id { get; init; }

    public DateOnly EffectiveDate { get; init; }

    public decimal Amount { get; init; }

    public DateIntervalType IntervalType { get; set; } = null!;

    public int Frequency { get; set; }

    public int Recurrence { get; set; }

    public string Description { get; init; } = null!;

    public bool Inactive { get; init; }

    public ImmutableArray<string> Tags
    {
        get => _tags;
        init => _tags = ImmutableArray.Create(value.ToArray());
    }

    public CategoryInfoViewModel Category { get; init; } = null!;

    public AccountInfoViewModel Account { get; init; } = null!;
}
