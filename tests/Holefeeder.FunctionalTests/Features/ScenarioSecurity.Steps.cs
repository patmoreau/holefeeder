using System.Net;

using DrifterApps.Seeds.Testing.Infrastructure;
using DrifterApps.Seeds.Testing.Scenarios;

using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.FunctionalTests.Features;

public partial class FeatureSecurity
{
    private void TheResourceIsInvoked(IStepRunner runner, ApiResource apiResources) =>
        runner.Execute("I invoke the resource", async () =>
        {
            if (apiResources.HttpMethod == HttpMethod.Post)
            {
                await HttpClientDriver.SendRequestAsync(apiResources);
                return;
            }

            if (apiResources.IsQuery())
            {
                await HttpClientDriver.SendRequestAsync(apiResources, string.Empty);
                return;
            }

            var parameters = Fakerizer.Make(apiResources.ParameterCount, () => Fakerizer.RandomGuid().ToString())
                .Cast<object>()
                .ToArray();

            await HttpClientDriver.SendRequestAsync(apiResources, parameters);
        });

    private void UserShouldBeAuthorizedToAccessEndpoint(IStepRunner runner) =>
        runner.Execute("the user should be authorized to access the endpoint", () =>
        {
            HttpClientDriver.ResponseStatusCode.Should().NotBe(HttpStatusCode.Unauthorized).And.NotBe(HttpStatusCode.Forbidden);
        });

    private void UserShouldNotBeAuthorizedToAccessEndpoint(IStepRunner runner) =>
        runner.Execute("the user should not be authorized to access the endpoint", () =>
        {
            HttpClientDriver.ResponseStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        });

    private void UserShouldBeForbiddenToAccessEndpoint(IStepRunner runner) =>
        runner.Execute("the user should be forbidden to access the endpoint", () =>
        {
            HttpClientDriver.ResponseStatusCode.Should().Be(HttpStatusCode.Forbidden);
        });
}
