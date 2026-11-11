using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Cashflows.Core.ClearCashflow;

internal sealed class ClearCashflowUseCase(ICashflows cashflows)
{
    public async Task<Result<Id>> HandleAsync(ClearCashflowRequest request, CancellationToken cancellationToken)
    {
        var cashflowResult = await cashflows.GetAsync(request.User, request.CashflowId, cancellationToken);
        if (cashflowResult.IsFailure)
        {
            return cashflowResult.Error;
        }
        var cashflow = cashflowResult.Value;

        var payment = cashflow.Pay(Id.New(), request.CashflowDate, Money.Zero, request.CashflowDate);

        var saveResult = await cashflows.SaveAsync(request.User, cashflow, payment, cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error;
        }

        return payment.Id;
    }
}
