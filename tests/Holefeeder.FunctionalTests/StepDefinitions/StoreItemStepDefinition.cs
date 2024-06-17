using System.Text.Json;

using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.Scenarios;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Application.Features.StoreItems.Queries;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Tests.Common.Builders.StoreItems;

using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class StoreItemStepDefinition(IHttpClientDriver httpClientDriver) : StepDefinition(httpClientDriver)
{
    private const string ContextId = $"{nameof(StoreItemStepDefinition)}_Id";
    private const string ContextCreateStoreItemRequest = $"{nameof(StoreItemStepDefinition)}_CreateStoreItemRequest";
    private const string ContextModifyStoreItemRequest = $"{nameof(StoreItemStepDefinition)}_ModifyStoreItemRequest";

    public void GetsCreated(IStepRunner runner) =>
        runner.Execute("a store item is created", async () =>
        {
            var request = new CreateStoreItemRequestBuilder().Build();
            runner.SetContextData(ContextCreateStoreItemRequest, request);

            var json = JsonSerializer.Serialize(request);
            await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.CreateStoreItem, json);

            var id = WithCreatedId();

            runner.SetContextData(ContextId, id);
        });

    public void Exists(IStepRunner runner) => Exists(runner, default);

    public void Exists(IStepRunner runner, CreateStoreItem.Request? request) =>
        runner.Execute("a store item is created", async () =>
        {
            var json = JsonSerializer.Serialize(request ?? new CreateStoreItemRequestBuilder().Build());
            await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.CreateStoreItem, json);

            var id = WithCreatedId();

            runner.SetContextData(ContextId, id);
        });

    public void GetsModified(IStepRunner runner) =>
        runner.Execute($"a {nameof(ApiResources.ModifyStoreItem)} request is sent", async () =>
        {
            var request = GivenAModifyStoreItemRequest().WithId(WithCreatedId()).Build();
            runner.SetContextData(ContextModifyStoreItemRequest, request);

            var json = JsonSerializer.Serialize(request);
            await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.ModifyStoreItem, json);
        });

    public void ShouldMatchTheModificationRequest(IStepRunner runner) =>
        runner.Execute("the StoreItem should match the request", async () =>
        {
            var id = runner.GetContextData<Guid>(ContextId);
            var request = runner.GetContextData<ModifyStoreItem.Request>(ContextModifyStoreItemRequest);

            await RetrievedById(id);
            WithResultAs<StoreItemViewModel>().Should().BeEquivalentTo(request);
        });

    public void ShouldMatchTheCreationRequest(IStepRunner runner) =>
        runner.Execute("the StoreItem should match the request", async () =>
        {
            var id = runner.GetContextData<Guid>(ContextId);
            var request = runner.GetContextData<CreateStoreItem.Request>(ContextCreateStoreItemRequest);

            await RetrievedById(id);
            WithResultAs<StoreItemViewModel>().Should().BeEquivalentTo(request);
        });

    private Task RetrievedById(Guid id) => HttpClientDriver.SendRequestAsync(ApiResources.GetStoreItem, id);
}
