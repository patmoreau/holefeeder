using System.Collections.Immutable;
using Holefeeder.Application.Features.Accounts.Queries;

namespace Holefeeder.Application.Models;

public record TransactionInfoViewModel
{
    private readonly ImmutableArray<string> _tags = ImmutableArray<string>.Empty;

    public Guid Id { get; init; }

    public DateOnly Date { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = null!;

    public ImmutableArray<string> Tags
    {
        get => _tags;
        init => _tags = ImmutableArray.Create(value.ToArray());
    }

    public CategoryInfoViewModel Category { get; init; } = null!;

    public AccountInfoViewModel Account { get; init; } = null!;
}
