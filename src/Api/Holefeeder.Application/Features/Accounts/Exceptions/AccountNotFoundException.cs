using Holefeeder.Application.Exceptions;
using Holefeeder.Domain.Features.Accounts;

namespace Holefeeder.Application.Features.Accounts.Exceptions;

#pragma warning disable CA1032
public class AccountNotFoundException(Guid id) : NotFoundException<Account>(id)
#pragma warning restore CA1032
{
}
