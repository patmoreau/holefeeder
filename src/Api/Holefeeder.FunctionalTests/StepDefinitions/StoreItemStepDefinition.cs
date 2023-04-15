using System.Text.Json;
using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common.Builders.StoreItems;
using Holefeeder.Tests.Common.SeedWork.Drivers;
using Holefeeder.Tests.Common.SeedWork.Scenarios;
using Holefeeder.Tests.Common.SeedWork.StepDefinitions;
using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class StoreItemStepDefinition : RootStepDefinition
{
    private const string ContextId = $"{nameof(StoreItemStepDefinition)}_Id";
    private const string ContextCreateStoreItemRequest = $"{nameof(StoreItemStepDefinition)}_CreateStoreItemRequest";
    private const string ContextModifyStoreItemRequest = $"{nameof(StoreItemStepDefinition)}_ModifyStoreItemRequest";

    public StoreItemStepDefinition(HttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
    }

    internal void GetsCreated(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute("a store item is created", async () =>
        {
            var request = new CreateStoreItemRequestBuilder().Build();
            runner.SetContextData(ContextCreateStoreItemRequest, request);

            string json = JsonSerializer.Serialize(request);
            await HttpClientDriver.SendPostRequest(ApiResources.CreateStoreItem, json);

            var id = WithCreatedId();

            runner.SetContextData(ContextId, id);
        });
    }

    internal void Exists(IStepRunner runner) => Exists(runner, default);

    internal void Exists(IStepRunner runner, CreateStoreItem.Request? request)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute("a store item is created", async () =>
        {
            string json = JsonSerializer.Serialize(request ?? new CreateStoreItemRequestBuilder().Build());
            await HttpClientDriver.SendPostRequest(ApiResources.CreateStoreItem, json);

            var id = WithCreatedId();

            runner.SetContextData(ContextId, id);
        });
    }

    internal void GetsModified(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute($"a {nameof(ApiResources.ModifyStoreItem)} request is sent", () =>
        {
            var request = GivenAModifyStoreItemRequest().WithId(WithCreatedId()).Build();
            runner.SetContextData(ContextModifyStoreItemRequest, request);

            string json = JsonSerializer.Serialize(request);
            return HttpClientDriver.SendPostRequest(ApiResources.ModifyStoreItem, json);
        });
    }

    internal void ShouldMatchTheModificationRequest(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute("the StoreItem should match the request", async () =>
        {
            var id = runner.GetContextData<Guid>(ContextId);
            var request = runner.GetContextData<ModifyStoreItem.Request>(ContextModifyStoreItemRequest);

            await RetrievedById(id);
            WithResultAs<StoreItemViewModel>().Should().BeEquivalentTo(request);
        });
    }

    internal void ShouldMatchTheCreationRequest(IStepRunner runner)
    {
        ArgumentNullException.ThrowIfNull(runner);

        runner.Execute("the StoreItem should match the request", async () =>
        {
            var id = runner.GetContextData<Guid>(ContextId);
            var request = runner.GetContextData<CreateStoreItem.Request>(ContextCreateStoreItemRequest);

            await RetrievedById(id);
            WithResultAs<StoreItemViewModel>().Should().BeEquivalentTo(request);
        });
    }

    private Task RetrievedById(Guid id) => HttpClientDriver.SendGetRequest(ApiResources.GetStoreItem, id);
}
