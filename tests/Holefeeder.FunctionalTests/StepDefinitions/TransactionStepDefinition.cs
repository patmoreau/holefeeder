using System.Text.Json;

using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

internal sealed class TransactionStepDefinition(IHttpClientDriver httpClientDriver) : StepDefinition(httpClientDriver)
{
    public async Task MakesPurchase(MakePurchase.Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.MakePurchase, json);
    }

    public async Task PayACashflow(PayCashflow.Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.PayCashflow, json);
    }

    public async Task Transfer(Transfer.Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.Transfer, json);
    }
}
