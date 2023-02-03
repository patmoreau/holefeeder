using System.Text.Json;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class TransactionStepDefinition
{
    public TransactionStepDefinition(HttpClientDriver httpClientDriver)
    {
        HttpClientDriver = httpClientDriver;
    }

    private HttpClientDriver HttpClientDriver { get; }

    internal async Task MakesPurchase(MakePurchase.Request request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.MakePurchase, json);
    }

    internal async Task PayACashflow(PayCashflow.Request request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.PayCashflow, json);
    }

    internal async Task Transfer(Transfer.Request request)
    {
        var json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResources.Transfer, json);
    }
}
