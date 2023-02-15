using System.Net;
using System.Text.Json;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.MyData.Commands.ImportData;
using static Holefeeder.Tests.Common.Builders.MyData.ImportDataRequestBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataAccountDtoBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataCashflowDtoBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataCategoryDtoBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataTransactionDtoBuilder;

namespace Holefeeder.FunctionalTests.Features.MyData;

public class ScenarioImportData : BaseScenario
{
    public ScenarioImportData(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnImportDataRequest()
            .WithNoData()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserImportsData(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var request = GivenAnImportDataRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserImportsData(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var request = GivenAnImportDataRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserImportsData(request);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var request = GivenAnImportDataRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserImportsData(request);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenDataIsImported()
    {
        var accounts = GivenMyAccountData().Build(2);
        var categories = GivenMyCategoryData().Build(2);
        var cashflows = GivenMyCashflowData()
            .WithAccount(accounts[0])
            .WithCategory(categories[0])
            .Build(2);
        var transactions = GivenMyTransactionData()
            .WithAccount(accounts[0])
            .WithCategory(categories[0])
            .Build(2);

        var request = GivenAnImportDataRequest()
            .WithUpdateExisting()
            .WithAccounts(accounts)
            .WithCategories(categories)
            .WithCashflows(cashflows)
            .WithTransactions(transactions)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserImportsData(request);

        ThenShouldExpectStatusCode(HttpStatusCode.Accepted);

        var id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader();

        var dto = await ThenWaitForCompletion(id);

        ThenAssertAll(async () =>
        {
            dto.Should().NotBeNull("The import task never completed");
            dto!.Status.Should().NotBe(CommandStatus.Error, dto.Message);

            await AssertAccount(accounts[0]);
            await AssertAccount(accounts[1]);
            await AssertCategory(categories[0]);
            await AssertCategory(categories[1]);
            await AssertCashflow(cashflows[0]);
            await AssertCashflow(cashflows[1]);
            await AssertTransaction(transactions[0]);
            await AssertTransaction(transactions[1]);
        });

        async Task AssertAccount(MyDataAccountDto account)
        {
            var result = await DatabaseDriver.FindByIdAsync<Account>(account.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(account);
        }

        async Task AssertCategory(MyDataCategoryDto category)
        {
            var result = await DatabaseDriver.FindByIdAsync<Category>(category.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(category);
        }

        async Task AssertCashflow(MyDataCashflowDto cashflow)
        {
            var result = await DatabaseDriver.FindByIdAsync<Cashflow>(cashflow.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(cashflow);
        }

        async Task AssertTransaction(MyDataTransactionDto transaction)
        {
            var result = await DatabaseDriver.FindByIdAsync<Transaction>(transaction.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(transaction);
        }
    }

    private async Task WhenUserImportsData(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ImportData, json);
    }

    private async Task<ImportDataStatusDto?> ThenWaitForCompletion(Guid importId)
    {
        var tries = 0;
        var inProgress = true;
        const int retryDelayInSeconds = 5;
        const int numberOfRetry = 10;

        while (tries < numberOfRetry && inProgress)
        {
            await Task.Delay(TimeSpan.FromSeconds(retryDelayInSeconds));

            await HttpClientDriver.SendGetRequest(ApiResources.ImportDataStatus, importId);

            ThenShouldExpectStatusCode(HttpStatusCode.OK);

            var dto = HttpClientDriver.DeserializeContent<ImportDataStatusDto>();
            dto.Should().NotBeNull();
            if (dto!.Status == CommandStatus.Completed)
            {
                return dto;
            }
            else if (dto.Status == CommandStatus.Error)
            {
                return dto;
            }

            tries++;
        }

        return null;
    }
}
