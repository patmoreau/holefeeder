using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Cashflows.Core.CancelCashflow;

internal sealed class CancelCashflowUseCase(ICashflows cashflows)
{
    public async Task<Result<Nothing>> HandleAsync(CancelCashflowRequest request, CancellationToken cancellationToken)
    {
        var cashflowResult = await cashflows.GetAsync(request.User, request.CashflowId, cancellationToken);
        if (cashflowResult.IsFailure)
        {
            return cashflowResult.Error;
        }

        var cancelled = cashflowResult.Value.Cancel();
        if (cancelled.IsFailure)
        {
            return cancelled.Error;
        }

        return await cashflows.SaveAsync(request.User, cancelled.Value, null, cancellationToken);
    }
}
