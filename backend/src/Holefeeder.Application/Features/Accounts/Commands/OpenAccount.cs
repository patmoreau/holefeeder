using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.Features.Accounts.Commands;

public static class OpenAccount
{
    public record Request(AccountType Type, string Name, DateOnly OpenDate, Money OpenBalance, string Description);
}
