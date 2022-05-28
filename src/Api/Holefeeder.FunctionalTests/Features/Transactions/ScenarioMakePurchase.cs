using System.Net;
using System.Text.Json;

using FluentAssertions;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Mapping;

using Xunit;

using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.Tests.Common.Builders.CategoryEntityBuilder;
using static Holefeeder.Tests.Common.Builders.TransactionBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioMakePurchase : BaseScenario
{
    private readonly TransactionMapper _mapper = new(new TagsMapper(), new AccountMapper(), new CategoryMapper());
    private readonly HolefeederDatabaseDriver _databaseDriver;

    public ScenarioMakePurchase(ApiApplicationDriver apiApplicationDriver) : base(apiApplicationDriver)
    {
        _databaseDriver = apiApplicationDriver.CreateHolefeederDatabaseDriver();
        _databaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var entity = GivenATransaction()
            .OfAmount(0)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserMakesAPurchase(entity);

        ThenShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var entity = GivenATransaction().Build();

        GivenUserIsAuthorized();

        await WhenUserMakesAPurchase(entity);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var entity = GivenATransaction().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserMakesAPurchase(entity);

        ThenUserShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var entity = GivenATransaction().Build();

        GivenUserIsUnauthorized();

        await WhenUserMakesAPurchase(entity);

        ThenUserShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenMakePurchase()
    {
        var account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        var category = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .SavedInDb(_databaseDriver);

        var entity = GivenATransaction()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(AuthorizedUserId)
            .Build();

        GivenUserIsAuthorized();

        await WhenUserMakesAPurchase(entity);

        ThenShouldExpectStatusCode(HttpStatusCode.Created);

        var id = ThenShouldGetTheRouteOfTheNewResourceInTheHeader();

        var result = await _databaseDriver.FindByIdAsync<TransactionEntity>(id, entity.UserId);

        _mapper.MapToModelOrNull(result).Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(entity, options => options.Excluding(info => info.Id));
    }

    private async Task WhenUserMakesAPurchase(Transaction entity)
    {
        var json = JsonSerializer.Serialize(new
        {
            entity.Date,
            entity.Amount,
            entity.Description,
            entity.AccountId,
            entity.CategoryId,
            entity.Tags
        });
        await HttpClientDriver.SendPostRequest(ApiResources.MakePurchase, json);
    }
}
