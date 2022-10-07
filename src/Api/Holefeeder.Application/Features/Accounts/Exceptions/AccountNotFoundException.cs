using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Application.Features.Accounts.Exceptions;

public class AccountNotFoundException : DomainException
{
    public AccountNotFoundException(Guid id) : base(StatusCodes.Status404NotFound,
        $"{nameof(Account)} '{id}' not found")
    {
    }

    public AccountNotFoundException()
    {
    }

    public AccountNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AccountNotFoundException(string message) : base(message)
    {
    }

    public override string Context => nameof(AccountNotFoundException);
}
