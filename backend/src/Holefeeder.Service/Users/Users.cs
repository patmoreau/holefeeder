using Holefeeder.Service.Users.Api.GetCurrentUser;
using Holefeeder.Service.Users.Core;
using Holefeeder.Service.Users.Core.GetCurrentUser;
using Holefeeder.Service.Users.Persistence;

namespace Holefeeder.Service.Users;

internal static class Users
{
    public static IServiceCollection AddUsersServices(this IServiceCollection services) =>
        services
            .AddScoped<IRegisteredUsers, RegisteredUsersInDatabase>()
            .AddScoped<GetCurrentUserUseCase>();

    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app) =>
        app.MapGetCurrentUser();
}
