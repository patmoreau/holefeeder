using System.Net;
using System.Text.Json;
using Holefeeder.Application;
using Holefeeder.Application.Features.MyData.Models;
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

[ComponentTest]
public class ScenarioImportData : HolefeederScenario
{
    public ScenarioImportData(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request request = GivenAnImportDataRequest()
            .WithNoData()
            .Build();

        GivenUserIsAuthorized();

        await WhenUserImportsData(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenDataIsImported()
    {
        var accounts = GivenMyAccountData().BuildCollection(2);
        var categories = GivenMyCategoryData().BuildCollection(2);
        var cashflows = GivenMyCashflowData()
            .WithAccount(accounts.ElementAt(0))
            .WithCategory(categories.ElementAt(0))
            .BuildCollection(2);
        var transactions = GivenMyTransactionData()
            .WithAccount(accounts.ElementAt(0))
            .WithCategory(categories.ElementAt(0))
            .BuildCollection(2);

        Request request = GivenAnImportDataRequest()
            .WithUpdateExisting()
            .WithAccounts(accounts.ToArray())
            .WithCategories(categories.ToArray())
            .WithCashflows(cashflows.ToArray())
            .WithTransactions(transactions.ToArray())
            .Build();

        GivenUserIsAuthorized();

        await WhenUserImportsData(request);

        ShouldExpectStatusCode(HttpStatusCode.Accepted);

        Uri location = ShouldGetTheRouteOfTheNewResourceInTheHeader();
        var id = ResourceIdFromLocation(location);

        ImportDataStatusDto? dto = await ThenWaitForCompletion(id);

        AssertAll(async () =>
        {
            dto.Should().NotBeNull("The import task never completed");
            dto!.Status.Should().NotBe(CommandStatus.Error, dto.Message);

            await AssertAccount(accounts.ElementAt(0));
            await AssertAccount(accounts.ElementAt(1));
            await AssertCategory(categories.ElementAt(0));
            await AssertCategory(categories.ElementAt(1));
            await AssertCashflow(cashflows.ElementAt(0));
            await AssertCashflow(cashflows.ElementAt(1));
            await AssertTransaction(transactions.ElementAt(0));
            await AssertTransaction(transactions.ElementAt(1));
        });

        async Task AssertAccount(MyDataAccountDto account)
        {
            Account? result = await DatabaseDriver.FindByIdAsync<Account>(account.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(account);
        }

        async Task AssertCategory(MyDataCategoryDto category)
        {
            Category? result = await DatabaseDriver.FindByIdAsync<Category>(category.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(category);
        }

        async Task AssertCashflow(MyDataCashflowDto cashflow)
        {
            Cashflow? result = await DatabaseDriver.FindByIdAsync<Cashflow>(cashflow.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(cashflow);
        }

        async Task AssertTransaction(MyDataTransactionDto transaction)
        {
            Transaction? result = await DatabaseDriver.FindByIdAsync<Transaction>(transaction.Id);
            result.Should().NotBeNull();
            result!.Should().BeEquivalentTo(transaction);
        }
    }

    private async Task WhenUserImportsData(Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequestAsync(ApiResources.ImportData, json);
    }

    private async Task<ImportDataStatusDto?> ThenWaitForCompletion(Guid importId)
    {
        int tries = 0;
        bool inProgress = true;
        const int retryDelayInSeconds = 5;
        const int numberOfRetry = 10;

        while (tries < numberOfRetry && inProgress)
        {
            await Task.Delay(TimeSpan.FromSeconds(retryDelayInSeconds));

            await HttpClientDriver.SendGetRequestAsync(ApiResources.ImportDataStatus, importId);

            ShouldExpectStatusCode(HttpStatusCode.OK);

            ImportDataStatusDto? dto = HttpClientDriver.DeserializeContent<ImportDataStatusDto>();
            dto.Should().NotBeNull();
            if (dto!.Status == CommandStatus.Completed)
            {
                return dto;
            }

            if (dto.Status == CommandStatus.Error)
            {
                return dto;
            }

            tries++;
        }

        return null;
    }
}
