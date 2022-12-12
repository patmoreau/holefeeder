using System.Text.Json;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common.Builders.StoreItems;

using Request = Holefeeder.Application.Features.StoreItems.Commands.CreateStoreItem.Request;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class StoreItemStepDefinition : BaseStepDefinition
{
    private readonly ModifyStoreItemRequestFactory _modifyStoreItemRequestFactory = new();

    public StoreItemStepDefinition(HttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
        HttpClientDriver = httpClientDriver;
    }

    private HttpClientDriver HttpClientDriver { get; }

    internal StoreItemStepDefinition GetsCreated(Request? request = null)
    {
        AddStep(() =>
        {
            var json = JsonSerializer.Serialize(request ?? new CreateStoreItemRequestBuilder().Build());
            return HttpClientDriver.SendPostRequest(ApiResources.CreateStoreItem, json);
        });

        return this;
    }

    internal StoreItemStepDefinition GetsModified(Application.Features.StoreItems.Commands.ModifyStoreItem.Request? request = null)
    {
        AddStep(() =>
        {
            var json = JsonSerializer.Serialize(request ?? _modifyStoreItemRequestFactory.Generate());
            return HttpClientDriver.SendPostRequest(ApiResources.ModifyStoreItem, json);
        });

        return this;
    }

    public StoreItemStepDefinition RetrievedById(Guid id)
    {
        AddStep(() => HttpClientDriver.SendGetRequest(ApiResources.GetStoreItem, id));

        return this;
    }
}
