using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Cashflows.Core.PayCashflow;

internal sealed class PayCashflowUseCase(ICashflows cashflows)
{
    public async Task<Result<Id>> HandleAsync(PayCashflowRequest request, CancellationToken cancellationToken)
    {
        var cashflowResult = await cashflows.GetAsync(request.User, request.CashflowId, cancellationToken);
        if (cashflowResult.IsFailure)
        {
            return cashflowResult.Error;
        }
        var cashflow = cashflowResult.Value;

        var payment = cashflow.Pay(Id.New(), request.Date, request.Amount, request.CashflowDate);
        var saved = request.UpdateRecurringAmount ? cashflow with { Amount = request.Amount } : cashflow;

        var saveResult = await cashflows.SaveAsync(request.User, saved, payment, cancellationToken);
        if (saveResult.IsFailure)
        {
            return saveResult.Error;
        }

        return payment.Id;
    }
}
