using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.Domain.Features.Users;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common.Builders.Users;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class UserStepDefinition(IHttpClientDriver httpClientDriver, BudgetingDatabaseDriver databaseDriver) : StepDefinition(httpClientDriver)
{
    public static readonly Guid HolefeederUserId = Fakerizer.RandomGuid();
    private static readonly string IdentityObjectId = Fakerizer.Random.Hash();

    public void IsUnauthorized(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is unauthorized", () => HttpClientDriver.UnAuthenticate());
    }

    public void IsAuthorized(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is authorized", async () =>
        {
            var user = await UserBuilder.GivenAUser()
                .WithId(HolefeederUserId)
                .SavedInDbAsync(databaseDriver);
            await UserIdentityBuilder.GivenAUserIdentity(user)
                .WithIdentityObjectId(IdentityObjectId)
                .SavedInDbAsync(databaseDriver);
            HttpClientDriver.AuthenticateUser(IdentityObjectId);
        });
    }

    public void IsForbidden(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);
        runner.Execute("the user is unauthorized",
            () => HttpClientDriver.AuthenticateUser(Fakerizer.Random.Hash()));
    }
}
