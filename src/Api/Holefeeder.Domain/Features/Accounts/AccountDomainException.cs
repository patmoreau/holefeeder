using Holefeeder.Domain.SeedWork;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.Domain.Features.Accounts;

#pragma warning disable CA1032
public class AccountDomainException : DomainException<Account>
#pragma warning restore CA1032
{
    public AccountDomainException(string message) : base(message)
    {
    }

    public AccountDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
