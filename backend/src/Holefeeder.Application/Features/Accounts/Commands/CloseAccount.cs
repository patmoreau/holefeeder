using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts.Commands;

public static class CloseAccount
{
    public record Request(AccountId Id);
}
