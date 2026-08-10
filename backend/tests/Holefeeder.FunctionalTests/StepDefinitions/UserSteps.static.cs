using Holefeeder.Domain.Features.Users;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal partial class UserSteps
{
    internal const string AuthorizedUser = nameof(AuthorizedUser);
    internal const string ForbiddenUser = nameof(ForbiddenUser);
    internal const string UnregisteredUser = nameof(UnregisteredUser);
    internal static IReadOnlyDictionary<string, TestUser> TestUsers { get; } = new Dictionary<string, TestUser>
    {
        [AuthorizedUser] = new()
        {
            UserId = (UserId)Fakerizer.RandomGuid(),
            IdentityObjectId = Fakerizer.Random.Hash(),
            Scope = "read:user write:user"
        },
        [ForbiddenUser] = new()
        {
            UserId = (UserId)Fakerizer.RandomGuid(),
            IdentityObjectId = Fakerizer.Random.Hash(),
            Scope = "forbidden.scope"
        },
        // Deliberately never saved to the database: this is the first-time caller.
        [UnregisteredUser] = new()
        {
            UserId = (UserId)Fakerizer.RandomGuid(),
            IdentityObjectId = Fakerizer.Random.Hash(),
            Scope = "read:user write:user"
        }
    };
}

internal sealed record TestUser
{
    public required UserId UserId { get; init; }
    public required string IdentityObjectId { get; init; }
    public required string Scope { get; init; }
}
