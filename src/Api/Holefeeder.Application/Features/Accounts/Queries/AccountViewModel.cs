using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts.Queries;

public record AccountViewModel(
    Guid Id,
    AccountType Type,
    string Name,
    decimal OpenBalance,
    DateTime OpenDate,
    int TransactionCount,
    decimal Balance,
    DateTime? Updated,
    string Description,
    bool Favorite,
    bool Inactive
);
