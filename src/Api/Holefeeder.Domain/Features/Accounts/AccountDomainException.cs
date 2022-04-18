using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.Accounts;

public class AccountDomainException : DomainException
{
    private AccountDomainException(string message) : base(StatusCodes.Status422UnprocessableEntity, message)
    {
    }

    public override string Context => nameof(AccountDomainException);

    public static AccountDomainException Create<T>(string message) where T : IAggregateRoot
    {
        return new AccountDomainException($"{typeof(T).Name} entity error: {message}");
    }
}
