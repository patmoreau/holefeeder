using Holefeeder.Domain.Features.Users;
using Holefeeder.Domain.ValueObjects;

namespace Holefeeder.Application.UserContext;

internal interface IUserContext
{
    UserId Id { get; }

    UserSettings Settings { get; }
}

