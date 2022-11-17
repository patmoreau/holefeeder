using Holefeeder.Domain.SeedWork;

namespace Holefeeder.Domain.Features.Accounts;

public class AccountDomainException : DomainException<Account>
{
    public AccountDomainException()
    {
    }

    public AccountDomainException(string message) : base(message)
    {
    }

    public AccountDomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
