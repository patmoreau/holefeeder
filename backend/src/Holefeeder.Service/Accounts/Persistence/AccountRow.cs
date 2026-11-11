using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Accounts.Persistence;

internal sealed record AccountRow(
    Guid Id,
    string Type,
    string Name,
    DateOnly OpenDate,
    decimal OpenBalance,
    string? Description,
    bool Favorite,
    bool Inactive)
{
    public Account ToAccount() =>
        new(
            Shared.Core.Id.Valid(Id),
            AccountType.Valid(Type),
            AccountName.Valid(Name),
            OpenDate,
            Money.Valid(OpenBalance),
            Description ?? string.Empty,
            Favorite,
            Inactive);
}
