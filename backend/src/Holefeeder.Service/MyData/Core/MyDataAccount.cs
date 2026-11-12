using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.MyData.Core;

internal sealed record MyDataAccount(
    Id Id,
    AccountType Type,
    string Name,
    Money OpenBalance,
    DateOnly OpenDate,
    string Description,
    bool Favorite,
    bool Inactive);
