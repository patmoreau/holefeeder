using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Cashflows.Core.ListCashflows;

internal sealed class ListCashflowsUseCase(ICashflows cashflows)
{
    public Task<Result<IReadOnlyList<CashflowSummary>>> HandleAsync(ListCashflowsRequest request, CancellationToken cancellationToken) =>
        cashflows.ListAsync(request.User, cancellationToken);
}
