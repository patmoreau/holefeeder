using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.MyData.Models;

public record MyDataAccountDto
{
    public Guid Id { get; init; }

    public AccountType Type { get; init; } = null!;

    public string Name { get; init; } = null!;

    public decimal OpenBalance { get; init; }

    public DateTime OpenDate { get; init; }

    public string Description { get; init; } = null!;

    public bool Favorite { get; init; }

    public bool Inactive { get; init; }
}
