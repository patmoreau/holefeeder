using System.Net;
using System.Text.Json;

using AutoBogus;

using Bogus;

using FluentAssertions;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Entities;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Application.Features.Transactions.Commands.ModifyCashflow;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.Tests.Common.Builders.CategoryEntityBuilder;
using static Holefeeder.Tests.Common.Builders.CashflowEntityBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioModifyCashflow : BaseScenario
{
    private readonly HolefeederDatabaseDriver _databaseDriver;

    public ScenarioModifyCashflow(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _databaseDriver = apiApplicationDriver.CreateHolefeederDatabaseDriver();
        _databaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var entity = GivenACashflowEntity()
            .OfAmount(Decimal.MinusOne)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedACashflow(MapToRequest(entity));

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var entity = GivenACashflowEntity().Build();

        GivenUserIsAuthorized();

        await WhenUserModifiedACashflow(MapToRequest(entity));

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenACashflowEntity().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserModifiedACashflow(MapToRequest(entity));

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenACashflowEntity().Build();

        GivenUserIsUnauthorized();

        await WhenUserModifiedACashflow(MapToRequest(entity));

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenModifyACashflow()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        var category = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        var cashflow = await GivenACashflowEntity()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        GivenUserIsAuthorized();

        cashflow = cashflow with
        {
            Amount = new Faker().Finance.Amount(),
            Description = AutoFaker.Generate<string>()
        };

        await WhenUserModifiedACashflow(MapToRequest(cashflow));

        ThenShouldExpectStatusCode(HttpStatusCode.NoContent);

        var result = await _databaseDriver.FindByIdAsync<CashflowEntity>(cashflow.Id, cashflow.UserId);

        result.Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(cashflow, options =>
                options.Excluding(info => info.LastPaidDate)
                    .Excluding(info => info.LastCashflowDate)
                    .Excluding(info => info.Account)
                    .Excluding(info => info.Category));
    }

    private async Task WhenUserModifiedACashflow(Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.ModifyCashflow, json);
    }

    private static Request MapToRequest(dynamic cashflow) =>
        new()
        {
            Id = cashflow.Id,
            Amount = cashflow.Amount,
            Description = cashflow.Description,
            Tags = cashflow.Tags is string ? cashflow.Tags.Split(";") : cashflow.Tags.ToArray()
        };
}
