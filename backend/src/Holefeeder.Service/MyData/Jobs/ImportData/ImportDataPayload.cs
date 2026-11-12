using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.MyData.Jobs.ImportData;

internal sealed record ImportDataPayload(
    Guid ImportId,
    Guid UserId,
    string IdentityObjectId,
    bool UpdateExisting,
    ImportAccountPayload[] Accounts,
    ImportCategoryPayload[] Categories,
    ImportCashflowPayload[] Cashflows,
    ImportTransactionPayload[] Transactions)
{
    public static ImportDataPayload From(Id importId, ImportDataRequest request) =>
        new(
            importId.Value,
            request.User.Id.Value,
            request.User.IdentityObjectId.Value,
            request.UpdateExisting,
            [.. request.Data.Accounts.Select(ImportAccountPayload.From)],
            [.. request.Data.Categories.Select(ImportCategoryPayload.From)],
            [.. request.Data.Cashflows.Select(ImportCashflowPayload.From)],
            [.. request.Data.Transactions.Select(ImportTransactionPayload.From)]);

    public ImportDataRequest ToRequest() =>
        new(
            User.Valid(Id.Valid(UserId), Support.Core.IdentityObjectId.Valid(IdentityObjectId)),
            UpdateExisting,
            new MyDataSet(
                [.. Accounts.Select(account => account.ToRecord())],
                [.. Categories.Select(category => category.ToRecord())],
                [.. Cashflows.Select(cashflow => cashflow.ToRecord())],
                [.. Transactions.Select(transaction => transaction.ToRecord())]));
}

internal sealed record ImportAccountPayload(
    Guid Id, string Type, string Name, decimal OpenBalance, DateOnly OpenDate, string Description, bool Favorite, bool Inactive)
{
    public static ImportAccountPayload From(MyDataAccount account) =>
        new(account.Id.Value, account.Type.Name, account.Name, account.OpenBalance.Value, account.OpenDate, account.Description,
            account.Favorite, account.Inactive);

    public MyDataAccount ToRecord() =>
        new(Shared.Core.Id.Valid(Id), AccountType.Valid(Type), Name, Money.Valid(OpenBalance), OpenDate, Description, Favorite, Inactive);
}

internal sealed record ImportCategoryPayload(
    Guid Id, string Type, string Name, string Color, decimal BudgetAmount, bool Favorite, bool System, bool Inactive)
{
    public static ImportCategoryPayload From(MyDataCategory category) =>
        new(category.Id.Value, category.Type.ToString(), category.Name, category.Color, category.BudgetAmount.Value, category.Favorite,
            category.System, category.Inactive);

    public MyDataCategory ToRecord() =>
        new(Shared.Core.Id.Valid(Id), Enum.Parse<CategoryType>(Type), Name, Color, Money.Valid(BudgetAmount), Favorite, System, Inactive);
}

internal sealed record ImportCashflowPayload(
    Guid Id,
    DateOnly EffectiveDate,
    decimal Amount,
    string IntervalType,
    int Frequency,
    int Recurrence,
    string Description,
    string Tags,
    Guid CategoryId,
    Guid AccountId,
    bool Inactive)
{
    public static ImportCashflowPayload From(MyDataCashflow cashflow) =>
        new(cashflow.Id.Value, cashflow.EffectiveDate, cashflow.Amount.Value, cashflow.IntervalType.Name, cashflow.Frequency,
            cashflow.Recurrence, cashflow.Description, cashflow.Tags.Text, cashflow.CategoryId.Value, cashflow.AccountId.Value,
            cashflow.Inactive);

    public MyDataCashflow ToRecord() =>
        new(Shared.Core.Id.Valid(Id), EffectiveDate, Money.Valid(Amount), Shared.Core.IntervalType.Valid(IntervalType), Frequency,
            Recurrence, Description, TagList.Create(Tags.Split(',')), Shared.Core.Id.Valid(CategoryId), Shared.Core.Id.Valid(AccountId),
            Inactive);
}

internal sealed record ImportTransactionPayload(
    Guid Id,
    DateOnly Date,
    decimal Amount,
    string Description,
    string Tags,
    Guid CategoryId,
    Guid AccountId,
    Guid? CashflowId,
    DateOnly? CashflowDate)
{
    public static ImportTransactionPayload From(MyDataTransaction transaction) =>
        new(transaction.Id.Value, transaction.Date, transaction.Amount.Value, transaction.Description, transaction.Tags.Text,
            transaction.CategoryId.Value, transaction.AccountId.Value, transaction.CashflowId?.Value, transaction.CashflowDate);

    public MyDataTransaction ToRecord() =>
        new(Shared.Core.Id.Valid(Id), Date, Money.Valid(Amount), Description, TagList.Create(Tags.Split(',')),
            Shared.Core.Id.Valid(CategoryId), Shared.Core.Id.Valid(AccountId),
            CashflowId is { } cashflowId ? Shared.Core.Id.Valid(cashflowId) : null, CashflowDate);
}
