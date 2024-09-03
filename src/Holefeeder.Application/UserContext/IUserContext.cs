using Holefeeder.Domain.Features.Users;

namespace Holefeeder.Application.UserContext;

internal interface IUserContext
{
    UserId Id { get; }
}

