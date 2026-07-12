using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.UseCases;
using Holefeeder.Domain.Features.Categories;

using Refit;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed partial class CategorySteps
{
    [AssertionMethod]
    internal void ShouldBeSynced(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the category should be synced", async response =>
        {
            var request = runner.GetContextData<PowerSync.Request>(RequestContext.CurrentRequest);

            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Categories.FindAsync((CategoryId) request.Operations.First().Id);

            if (request.Operations.First().Op == "DELETE")
            {
                result.Should().NotBeNull();
                result!.Inactive.Should().BeTrue();
                return;
            }

            var expected = runner.GetContextData<Category>(PowerSyncContext.SyncData);
            result.Should().NotBeNull();
            result!.Id.Should().Be(expected.Id);
            result.Type.Should().Be(expected.Type);
            result.Name.Should().Be(expected.Name);
            result.Color.Should().Be(expected.Color);
            result.Favorite.Should().Be(expected.Favorite);
            result.System.Should().Be(expected.System);
            result.Inactive.Should().Be(expected.Inactive);
            result.BudgetAmount.Should().Be(expected.BudgetAmount);
        });
}
