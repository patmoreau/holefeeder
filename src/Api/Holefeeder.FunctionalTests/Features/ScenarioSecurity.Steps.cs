using System.Net;

using DrifterApps.Seeds.Testing.Infrastructure;

namespace Holefeeder.FunctionalTests.Features;

public partial class FeatureSecurity
{
    private Task When_I_invoke_the_resource(ApiResource apiResources)
    {
        if (apiResources.HttpMethod == HttpMethod.Post)
        {
            return HttpClientDriver.SendPostRequestAsync(apiResources);
        }

        var parameters = Fakerizer.Make(apiResources.ParameterCount, () => Fakerizer.Random.Guid().ToString())
            .Cast<object>()
            .ToArray();

        return apiResources.HttpMethod == HttpMethod.Delete ? HttpClientDriver.SendDeleteRequestAsync(apiResources, parameters) : HttpClientDriver.SendGetRequestAsync(apiResources, parameters);
    }

    private Task Then_user_should_be_authorized_to_access_endpoint()
    {
        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.NotHaveStatusCode(HttpStatusCode.Unauthorized).And.NotHaveStatusCode(HttpStatusCode.Forbidden);
        return Task.CompletedTask;
    }

    private Task Then_user_should_not_be_authorized_to_access_endpoint()
    {
        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.HaveStatusCode(HttpStatusCode.Unauthorized);
        return Task.CompletedTask;
    }

    private Task Then_user_should_be_forbidden_to_access_endpoint()
    {
        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.HaveStatusCode(HttpStatusCode.Forbidden);
        return Task.CompletedTask;
    }
}
