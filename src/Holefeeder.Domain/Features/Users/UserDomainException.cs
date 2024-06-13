namespace Holefeeder.Domain.Features.Users;

#pragma warning disable CA1032
public class UserDomainException(string message) : DomainException<User>(message)
#pragma warning restore CA1032
{
}
