using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.Accounts;

public class AccountDomainException : DomainException
{
    public AccountDomainException(string message) : base(StatusCodes.Status422UnprocessableEntity, message)
    {
    }

    public AccountDomainException()
    {
    }

    public AccountDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public override string Context => nameof(Account);
}
