using Holefeeder.Service.MyData.Core;

namespace Holefeeder.Service.MyData.Api.GetImportStatus;

internal sealed record ImportStatusResponse(
    string Status,
    int AccountsProcessed,
    int AccountsTotal,
    int CashflowsProcessed,
    int CashflowsTotal,
    int CategoriesProcessed,
    int CategoriesTotal,
    int TransactionsProcessed,
    int TransactionsTotal,
    string Message)
{
    public static ImportStatusResponse From(ImportStatus status) =>
        new(status.State.ToString(), status.Accounts.Processed, status.Accounts.Total, status.Cashflows.Processed, status.Cashflows.Total,
            status.Categories.Processed, status.Categories.Total, status.Transactions.Processed, status.Transactions.Total, status.Message);
}
