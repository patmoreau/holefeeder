using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;
using DrifterApps.Seeds.Testing.Extensions;

using Holefeeder.Application.Features.StoreItems.Commands;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class StoreItemSteps(BudgetingDatabaseDriver budgetingDatabaseDriver)
{
    public void Exists(IStepRunner runner) =>
        runner.Execute("a store item exists", async () =>
        {
            var item = await GivenAStoreItem()
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .SavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(StoreItemContext.ExistingStoreItem, item);

            return item;
        });

    public void CollectionExists(IStepRunner runner) =>
        runner.Execute("the user has multiple items in the store", async () =>
        {
            var builder = GivenAStoreItem().ForUser(TestUsers[AuthorizedUser].UserId);

            var items = await builder.CollectionSavedInDbAsync(budgetingDatabaseDriver);

            runner.SetContextData(StoreItemContext.ExistingStoreItems, items);

            return items;
        });

    [AssertionMethod]
    public void ShouldBeCreated(IStepRunner runner) =>
        runner.Execute<IApiResponse, Task>("the new store item should be created", async response =>
        {
            var request = runner.GetContextData<CreateStoreItem.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.Created);

            response.Value.Headers.Location.Should().NotBeNull();

            var id = (StoreItemId)response.Value.Headers.Location!.ExtractGuidFromUrl();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.StoreItems.FindAsync(id);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        });

    [AssertionMethod]
    public void ShouldBeModified(IStepRunner runner) =>
        runner.Execute<IApiResponse, Task>("the new store item should be modified", async response =>
        {
            var request = runner.GetContextData<ModifyStoreItem.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.StoreItems.FindAsync(request.Id);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(request, options => options.ExcludingMissingMembers());
        });
}
