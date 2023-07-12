using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts.Queries;

public record AccountViewModel(
    Guid Id,
    AccountType Type,
    string Name,
    decimal OpenBalance,
    DateOnly OpenDate,
    int TransactionCount,
    decimal Balance,
    DateOnly? Updated,
    string Description,
    bool Favorite,
    bool Inactive
);
