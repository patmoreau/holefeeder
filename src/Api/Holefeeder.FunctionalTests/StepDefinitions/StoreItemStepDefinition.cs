using System.Text.Json;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common.Builders.StoreItems;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class StoreItemStepDefinition : BaseStepDefinition
{
    public StoreItemStepDefinition(HttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
        HttpClientDriver = httpClientDriver;
    }

    private HttpClientDriver HttpClientDriver { get; }

    internal StoreItemStepDefinition GetsCreated(CreateStoreItem.Request? request = null)
    {
        AddStep(() =>
        {
            var json = JsonSerializer.Serialize(request ?? new CreateStoreItemRequestBuilder().Build());
            return HttpClientDriver.SendPostRequest(ApiResources.CreateStoreItem, json);
        });

        return this;
    }

    internal StoreItemStepDefinition GetsModified(ModifyStoreItem.Request? request = null)
    {
        AddStep(() =>
        {
            var json = JsonSerializer.Serialize(request ?? new ModifyStoreItemRequestBuilder().Build());
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
