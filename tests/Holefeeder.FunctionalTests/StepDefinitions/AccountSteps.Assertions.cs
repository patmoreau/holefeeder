using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;
using DrifterApps.Seeds.Testing.Extensions;

using Holefeeder.Domain.Features.Accounts;

using Refit;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed partial class AccountSteps
{
    [AssertionMethod]
    public void ShouldBeClosed(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the account should be closed", async response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            account.Should().NotBeNull();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Accounts.FindAsync(account.Id);
            result.Should().NotBeNull();
            result!.Inactive.Should().BeTrue();
        });

    [AssertionMethod]
    public void ShouldBeAFavorite(IStepRunner runner) =>
        runner.Execute<IApiResponse>("the account should be a favorite", async response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            account.Should().NotBeNull();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Accounts.FindAsync(account.Id);
            result.Should().NotBeNull();
            result!.Favorite.Should().BeTrue();
        });

    [AssertionMethod]
    public void NameShouldBeModified(IStepRunner runner, string name) =>
        runner.Execute<IApiResponse>("the account name should be modified", async response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NoContent);

            var account = runner.GetContextData<Account>(AccountContexts.ExistingAccount);
            account.Should().NotBeNull();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Accounts.FindAsync(account.Id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(name);
        });

    [AssertionMethod]
    public void ShouldBeCreated(IStepRunner runner, string name) =>
        runner.Execute<IApiResponse>("the new account should be created", async response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.Created);

            response.Value.Headers.Location.Should().NotBeNull();

            var id = (AccountId)response.Value.Headers.Location!.ExtractGuidFromUrl();

            await using var dbContext = budgetingDatabaseDriver.CreateDbContext();

            var result = await dbContext.Accounts.FindAsync(id);
            result.Should().NotBeNull();
            result!.Name.Should().Be(name);
        });
}
