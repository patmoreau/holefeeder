using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Models;

public record AccountViewModel
{
    public Guid Id { get; init; }

    public AccountType Type { get; init; } = null!;

    public string Name { get; init; } = null!;

    public decimal OpenBalance { get; init; }

    public DateOnly OpenDate { get; init; }

    public int TransactionCount { get; init; }

    public decimal Balance { get; init; }

    public DateOnly? Updated { get; init; }

    public string Description { get; init; } = null!;

    public bool Favorite { get; init; }

    public bool Inactive { get; init; }
}
