using System.Net;
using AutoBogus;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Features;

public partial class FeatureSecurity
{
    private Task When_I_invoke_the_resource(ApiResource apiResource)
    {
        if (apiResource.HttpMethod == HttpMethod.Post)
        {
            return HttpClientDriver.SendPostRequest(apiResource);
        }

        var parameters = AutoFaker.Generate<Guid>(apiResource.ParameterCount)
            .Select(x => x.ToString())
            .Cast<object>()
            .ToArray();

        return apiResource.HttpMethod == HttpMethod.Delete ?
            HttpClientDriver.SendDeleteRequest(apiResource, parameters) :
            HttpClientDriver.SendGetRequest(apiResource, parameters);
    }

    private Task Then_user_should_be_authorized_to_access_endpoint()
    {
        HttpClientDriver.ResponseMessage.Should().NotBeNull()
            .And.NotHaveStatusCode(HttpStatusCode.Unauthorized);
        return Task.CompletedTask;
    }

    private Task Then_user_should_not_be_authorized_to_access_endpoint()
    {
        HttpClientDriver.ShouldBeUnauthorized();
        return Task.CompletedTask;
    }

    private Task Then_user_should_be_forbidden_to_access_endpoint()
    {
        HttpClientDriver.ResponseMessage.Should().NotBeNull().And.HaveStatusCode(HttpStatusCode.Forbidden);
        return Task.CompletedTask;
    }
}
