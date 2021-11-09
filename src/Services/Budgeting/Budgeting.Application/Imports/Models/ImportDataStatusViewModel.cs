namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Models
{
    public record ImportDataStatusViewModel(
        int AccountsProcessed, int AccountsTotal,
        int CashflowsProcessed, int CashflowsTotal,
        int CategoriesProcessed, int CategoriesTotal,
        int TransactionsProcessed, int TransactionsTotal);
}
