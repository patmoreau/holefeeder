using System.Text.Json;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public class TransactionStepDefinition
{
    private HttpClientDriver HttpClientDriver { get; }

    public TransactionStepDefinition(HttpClientDriver httpClientDriver)
    {
        HttpClientDriver = httpClientDriver;
    }


    public async Task MakesPurchase(Transaction entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

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
