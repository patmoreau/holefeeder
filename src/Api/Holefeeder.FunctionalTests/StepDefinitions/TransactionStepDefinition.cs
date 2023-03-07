using System.Text.Json;
using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class TransactionStepDefinition
{
    public TransactionStepDefinition(HttpClientDriver httpClientDriver) => HttpClientDriver = httpClientDriver;

    private HttpClientDriver HttpClientDriver { get; }

    internal async Task MakesPurchase(MakePurchase.Request request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResource.MakePurchase, json);
    }

    internal async Task PayACashflow(PayCashflow.Request request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResource.PayCashflow, json);
    }

    internal async Task Transfer(Transfer.Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequest(ApiResource.Transfer, json);
    }
}
