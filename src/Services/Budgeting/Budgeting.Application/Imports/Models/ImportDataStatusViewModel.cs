using DrifterApps.Holefeeder.Framework.SeedWork.Application;

namespace DrifterApps.Holefeeder.Budgeting.Application.Imports.Models;

public record ImportDataStatusViewModel(
    CommandStatus Status,
    int AccountsProcessed = 0, int AccountsTotal = 0,
    int CashflowsProcessed = 0, int CashflowsTotal = 0,
    int CategoriesProcessed = 0, int CategoriesTotal = 0,
    int TransactionsProcessed = 0, int TransactionsTotal = 0,
    string Message = "")
{
    public static ImportDataStatusViewModel Init() => new(CommandStatus.Initializing);
}
