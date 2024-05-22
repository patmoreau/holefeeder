using System.Net;

using DrifterApps.Seeds.Testing.Infrastructure;

using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common.Extensions;

namespace Holefeeder.FunctionalTests.Features;

public partial class FeatureSecurity
{
    private Task When_I_invoke_the_resource(ApiResource apiResources)
    {
        if (apiResources.HttpMethod == HttpMethod.Post)
        {
            return HttpClientDriver.SendRequestAsync(apiResources);
        }

        if (apiResources.IsQuery())
        {
            return HttpClientDriver.SendRequestAsync(apiResources, string.Empty);
        }

        var parameters = Fakerizer.Make(apiResources.ParameterCount, () => Fakerizer.RandomGuid().ToString())
            .Cast<object>()
            .ToArray();

        return HttpClientDriver.SendRequestAsync(apiResources, parameters);
    }

    private Task Then_user_should_be_authorized_to_access_endpoint()
    {
        HttpClientDriver.ResponseStatusCode.Should().NotBe(HttpStatusCode.Unauthorized).And.NotBe(HttpStatusCode.Forbidden);
        return Task.CompletedTask;
    }

    private Task Then_user_should_not_be_authorized_to_access_endpoint()
    {
        HttpClientDriver.ResponseStatusCode.Should().Be(HttpStatusCode.Unauthorized);
        return Task.CompletedTask;
    }

    private Task Then_user_should_be_forbidden_to_access_endpoint()
    {
        HttpClientDriver.ResponseStatusCode.Should().Be(HttpStatusCode.Forbidden);
        return Task.CompletedTask;
    }
}
