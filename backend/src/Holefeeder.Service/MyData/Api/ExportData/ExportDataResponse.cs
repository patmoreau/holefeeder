using Holefeeder.Service.MyData.Core;

namespace Holefeeder.Service.MyData.Api.ExportData;

internal sealed record ExportDataResponse(
    IReadOnlyList<ExportedAccountResponse> Accounts,
    IReadOnlyList<ExportedCategoryResponse> Categories,
    IReadOnlyList<ExportedCashflowResponse> Cashflows,
    IReadOnlyList<ExportedTransactionResponse> Transactions)
{
    public static ExportDataResponse From(MyDataSet data) =>
        new(
            [.. data.Accounts.Select(ExportedAccountResponse.From)],
            [.. data.Categories.Select(ExportedCategoryResponse.From)],
            [.. data.Cashflows.Select(ExportedCashflowResponse.From)],
            [.. data.Transactions.Select(ExportedTransactionResponse.From)]);

    internal static string CamelCase(string name) => char.ToLowerInvariant(name[0]) + name[1..];
}

internal sealed record ExportedAccountResponse(
    Guid Id, string Type, string Name, decimal OpenBalance, DateOnly OpenDate, string Description, bool Favorite, bool Inactive)
{
    public static ExportedAccountResponse From(MyDataAccount account) =>
        new(account.Id.Value, ExportDataResponse.CamelCase(account.Type.Name), account.Name, account.OpenBalance.Value, account.OpenDate,
            account.Description, account.Favorite, account.Inactive);
}

internal sealed record ExportedCategoryResponse(
    Guid Id, string Type, string Name, string Color, decimal BudgetAmount, bool Favorite, bool System, bool Inactive)
{
    public static ExportedCategoryResponse From(MyDataCategory category) =>
        new(category.Id.Value, ExportDataResponse.CamelCase(category.Type.ToString()), category.Name, category.Color,
            category.BudgetAmount.Value, category.Favorite, category.System, category.Inactive);
}

internal sealed record ExportedCashflowResponse(
    Guid Id,
    DateOnly EffectiveDate,
    decimal Amount,
    string IntervalType,
    int Frequency,
    int Recurrence,
    string Description,
    IReadOnlyList<string> Tags,
    Guid CategoryId,
    Guid AccountId,
    bool Inactive)
{
    public static ExportedCashflowResponse From(MyDataCashflow cashflow) =>
        new(cashflow.Id.Value, cashflow.EffectiveDate, cashflow.Amount.Value, cashflow.IntervalType.Name.ToLowerInvariant(),
            cashflow.Frequency, cashflow.Recurrence, cashflow.Description, cashflow.Tags.Values, cashflow.CategoryId.Value,
            cashflow.AccountId.Value, cashflow.Inactive);
}

internal sealed record ExportedTransactionResponse(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string Description,
    IReadOnlyList<string> Tags,
    Guid CategoryId,
    Guid AccountId,
    Guid? CashflowId,
    DateOnly? CashflowDate)
{
    public static ExportedTransactionResponse From(MyDataTransaction transaction) =>
        new(transaction.Id.Value, transaction.Date, transaction.Amount.Value, transaction.Description, transaction.Tags.Values,
            transaction.CategoryId.Value, transaction.AccountId.Value, transaction.CashflowId?.Value, transaction.CashflowDate);
}
