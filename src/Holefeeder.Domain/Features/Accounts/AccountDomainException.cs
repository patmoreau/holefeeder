namespace Holefeeder.Domain.Features.Accounts;

#pragma warning disable CA1032
public class AccountDomainException(string message) : DomainException<Account>(message)
#pragma warning restore CA1032
{
}
