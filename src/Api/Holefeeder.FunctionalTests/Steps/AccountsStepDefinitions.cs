using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.Steps;

[Binding]
public class AccountsStepDefinitions
{
    private readonly HttpClientDriver _httpClientDriver;
    private readonly DatabaseDriver _databaseDriver;

    public AccountsStepDefinitions(HttpClientDriver httpClientDriver, DatabaseDriver databaseDriver)
    {
        _httpClientDriver = httpClientDriver;
        _databaseDriver = databaseDriver;
    }

    [When(@"I try to (GetAccounts?)")]
    public void WhenITryToGetAccounts(ApiResources apiResource) => _httpClientDriver.SendGetRequest(apiResource);

    [Given(@"the following accounts")]
    public async Task GivenTheFollowingItems(Table table)
    {
        await _databaseDriver.AddAccountsToDatabase(table);
    }
}
