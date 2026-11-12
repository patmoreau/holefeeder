using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Cashflows.Core.ModifyCashflow;

internal sealed class ModifyCashflowUseCase(ICashflows cashflows)
{
    public async Task<Result<Nothing>> HandleAsync(ModifyCashflowRequest request, CancellationToken cancellationToken)
    {
        var cashflowResult = await cashflows.GetAsync(request.User, request.CashflowId, cancellationToken);
        if (cashflowResult.IsFailure)
        {
            return cashflowResult.Error;
        }

        var modified = cashflowResult.Value.Modify(request.Amount, request.EffectiveDate, request.Description, request.Tags);
        if (modified.IsFailure)
        {
            return modified.Error;
        }

        return await cashflows.SaveAsync(request.User, modified.Value, null, cancellationToken);
    }
}
