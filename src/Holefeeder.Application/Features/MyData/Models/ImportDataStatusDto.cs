namespace Holefeeder.Application.Features.MyData.Models;

public record ImportDataStatusDto(
    CommandStatus Status,
    int AccountsProcessed = 0, int AccountsTotal = 0,
    int CashflowsProcessed = 0, int CashflowsTotal = 0,
    int CategoriesProcessed = 0, int CategoriesTotal = 0,
    int TransactionsProcessed = 0, int TransactionsTotal = 0,
    string Message = "")
{
    public static ImportDataStatusDto Init() => new(CommandStatus.Initializing);
}
