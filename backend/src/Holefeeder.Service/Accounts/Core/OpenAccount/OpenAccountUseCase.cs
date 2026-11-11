using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

using User = Holefeeder.Service.Support.Core.User;
using UserErrors = Holefeeder.Service.Support.Core.UserErrors;

namespace Holefeeder.Service.Accounts.Core.OpenAccount;

internal sealed class OpenAccountUseCase(IAccounts accounts)
{
    public async Task<Result<Id>> HandleAsync(OpenAccountRequest request, CancellationToken cancellationToken)
    {
        if (request.User == User.NoUser)
        {
            return UserErrors.NotRegistered;
        }

        var account = Account.Open(Id.New(), request.Type, request.Name, request.OpenDate, request.OpenBalance, request.Description);

        var saved = await accounts.SaveAsync(request.User, account, cancellationToken);
        if (saved.IsFailure)
        {
            return saved.Error;
        }

        return account.Id;
    }
}
