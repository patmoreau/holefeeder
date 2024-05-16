using System.Text.Json;

using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class TransactionStepDefinition(IHttpClientDriver httpClientDriver) : StepDefinition(httpClientDriver)
{
    internal async Task MakesPurchase(MakePurchase.Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.MakePurchase, json);
    }

    internal async Task PayACashflow(PayCashflow.Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.PayCashflow, json);
    }

    internal async Task Transfer(Transfer.Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendRequestWithBodyAsync(ApiResources.Transfer, json);
    }
}
