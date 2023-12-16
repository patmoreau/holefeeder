using System.Text.Json;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;
using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class TransactionStepDefinition : StepDefinition
{
    public TransactionStepDefinition(IHttpClientDriver httpClientDriver) : base(httpClientDriver)
    {
    }

    internal async Task MakesPurchase(MakePurchase.Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequestAsync(ApiResources.MakePurchase, json);
    }

    internal async Task PayACashflow(PayCashflow.Request request)
    {
        ArgumentNullException.ThrowIfNull(request);

        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequestAsync(ApiResources.PayCashflow, json);
    }

    internal async Task Transfer(Transfer.Request request)
    {
        string json = JsonSerializer.Serialize(request);
        await HttpClientDriver.SendPostRequestAsync(ApiResources.Transfer, json);
    }
}
