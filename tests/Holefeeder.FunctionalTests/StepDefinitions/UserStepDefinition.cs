using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class UserStepDefinition : StepDefinition
{
    internal const string AuthorizedUser = nameof(AuthorizedUser);
    internal const string ForbiddenUser = nameof(ForbiddenUser);
    internal static IReadOnlyDictionary<string, TestUser> TestUsers { get; } = new Dictionary<string, TestUser>
    {
        [AuthorizedUser] = new()
        {
            UserId = Fakerizer.RandomGuid(),
            IdentityObjectId = Fakerizer.Random.Hash(),
            Scope = "holefeeder.user"
        },
        [ForbiddenUser] = new()
        {
            UserId = Fakerizer.RandomGuid(),
            IdentityObjectId = Fakerizer.Random.Hash(),
            Scope = "forbidden.scope"
        }
    };

    internal UserStepDefinition(IHttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
    }

    public void IsUnauthorized(IStepRunner runner) =>
        runner.Execute("the user is unauthorized", () => HttpClientDriver.UnAuthenticate());

    public void IsAuthorized(IStepRunner runner) =>
        runner.Execute("the user is authorized",
            () => HttpClientDriver.AuthenticateUser(TestUsers[AuthorizedUser].IdentityObjectId));

    public void IsForbidden(IStepRunner runner) =>
        runner.Execute("the user is unauthorized",
            () => HttpClientDriver.AuthenticateUser(TestUsers[ForbiddenUser].IdentityObjectId));
}

internal sealed record TestUser
{
    public required Guid UserId { get; init; }
    public required string IdentityObjectId { get; init; }
    public required string Scope { get; init; }
}
