using System.Globalization;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.MyData.Core.ImportData;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.MyData.Api.ImportData;

internal sealed record ImportDataRequestRaw([FromBody] ImportDataBody Body)
{
    public static Result<ImportDataRequest> ToImportDataRequest(ImportDataRequestRaw raw, User user)
    {
        var validation = ResultAggregate.Create();
        var updateExisting = new Fields(string.Empty, validation).Flag(raw.Body.UpdateExisting, "updateExisting");

        var data = raw.Body.Data;
        if ((data?.Accounts?.Count ?? 0) + (data?.Categories?.Count ?? 0) + (data?.Cashflows?.Count ?? 0) +
            (data?.Transactions?.Count ?? 0) == 0)
        {
            validation.AddResult(ImportDataErrors.DataEmpty);
        }

        var accounts = Records(data?.Accounts, "accounts", ToAccount, validation);
        var categories = Records(data?.Categories, "categories", ToCategory, validation);
        var cashflows = Records(data?.Cashflows, "cashflows", ToCashflow, validation);
        var transactions = Records(data?.Transactions, "transactions", ToTransaction, validation);

        return validation.OnSuccess<ImportDataRequest>(() =>
            new ImportDataRequest(user, updateExisting, new MyDataSet(accounts, categories, cashflows, transactions)));
    }

    private static List<T> Records<TBody, T>(
        IReadOnlyList<TBody>? bodies, string name, Func<TBody, Fields, T?> toRecord, ResultAggregate validation) where T : class =>
        [.. (bodies ?? []).Select((body, index) => toRecord(body, new Fields($"{name}[{index}].", validation))).OfType<T>()];

    private static MyDataAccount? ToAccount(ImportAccountBody body, Fields fields)
    {
        var id = fields.Id(body.Id, "id");
        var type = fields.Parsed(AccountType.Create(body.Type), "type", "an account type");
        var openBalance = fields.Money(body.OpenBalance, "openBalance");
        var openDate = fields.Date(body.OpenDate, "openDate");
        var favorite = fields.Flag(body.Favorite, "favorite");
        var inactive = fields.Flag(body.Inactive, "inactive");

        return fields.IsValid
            ? fields.Valid(new MyDataAccount(id!, type!, body.Name ?? string.Empty, openBalance!, openDate!.Value,
                body.Description ?? string.Empty, favorite, inactive).Validated())
            : null;
    }

    private static MyDataCategory? ToCategory(ImportCategoryBody body, Fields fields)
    {
        var id = fields.Id(body.Id, "id");
        var type = fields.CategoryType(body.Type, "type");
        var budgetAmount = fields.Money(body.BudgetAmount, "budgetAmount");
        var favorite = fields.Flag(body.Favorite, "favorite");
        var system = fields.Flag(body.System, "system");
        var inactive = fields.Flag(body.Inactive, "inactive");

        return fields.IsValid
            ? fields.Valid(new MyDataCategory(id!, type!.Value, body.Name ?? string.Empty, body.Color ?? string.Empty, budgetAmount!,
                favorite, system, inactive).Validated())
            : null;
    }

    private static MyDataCashflow? ToCashflow(ImportCashflowBody body, Fields fields)
    {
        var id = fields.Id(body.Id, "id");
        var effectiveDate = fields.Date(body.EffectiveDate, "effectiveDate");
        var amount = fields.Money(body.Amount, "amount");
        var intervalType = fields.Parsed(IntervalType.Create(body.IntervalType), "intervalType", "an interval type");
        var frequency = fields.Number(body.Frequency, "frequency");
        var recurrence = fields.Number(body.Recurrence, "recurrence");
        var categoryId = fields.Id(body.CategoryId, "categoryId");
        var accountId = fields.Id(body.AccountId, "accountId");
        var inactive = fields.Flag(body.Inactive, "inactive");

        return fields.IsValid
            ? fields.Valid(new MyDataCashflow(id!, effectiveDate!.Value, amount!, intervalType!, frequency, recurrence,
                body.Description ?? string.Empty, TagList.Create(body.Tags), categoryId!, accountId!, inactive).Validated())
            : null;
    }

    private static MyDataTransaction? ToTransaction(ImportTransactionBody body, Fields fields)
    {
        var id = fields.Id(body.Id, "id");
        var date = fields.Date(body.Date, "date");
        var amount = fields.Money(body.Amount, "amount");
        var categoryId = fields.Id(body.CategoryId, "categoryId");
        var accountId = fields.Id(body.AccountId, "accountId");
        var cashflowId = body.CashflowId is null ? null : fields.Id(body.CashflowId, "cashflowId");
        var cashflowDate = body.CashflowDate is null ? null : fields.Date(body.CashflowDate, "cashflowDate");

        return fields.IsValid
            ? fields.Valid(new MyDataTransaction(id!, date!.Value, amount!, body.Description ?? string.Empty, TagList.Create(body.Tags),
                categoryId!, accountId!, cashflowId, cashflowDate).Validated())
            : null;
    }

    private sealed class Fields(string prefix, ResultAggregate validation)
    {
        public bool IsValid { get; private set; } = true;

        public Id? Id(string? value, string field) => Parsed(Shared.Core.Id.Create(value), field, "a non-empty id");

        public Money? Money(string? value, string field) =>
            value is null ? Shared.Core.Money.Zero : Parsed(Shared.Core.Money.Create(value), field, "an amount of zero or more");

        public DateOnly? Date(string? value, string field) =>
            DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? date
                : Invalid<DateOnly?>(field, "a date as YYYY-MM-DD");

        public int Number(string? value, string field) =>
            value is null ? 0 :
            int.TryParse(value, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number)
                ? number
                : Invalid<int>(field, "a whole number");

        public bool Flag(string? value, string field) =>
            value switch
            {
                null or "false" => false,
                "true" => true,
                _ => Invalid<bool>(field, "true or false")
            };

        public CategoryType? CategoryType(string? value, string field) =>
            Enum.GetValues<CategoryType>().Any(type => string.Equals(type.ToString(), value, StringComparison.OrdinalIgnoreCase))
                ? Enum.Parse<CategoryType>(value!, ignoreCase: true)
                : Invalid<CategoryType?>(field, "expense or gain");

        public T? Parsed<T>(Result<T> result, string field, string expected) where T : class =>
            result.IsSuccess ? result.Value : Invalid<T?>(field, expected);

        public T? Valid<T>(Result<T> result) where T : class
        {
            if (result.IsSuccess)
            {
                return result.Value;
            }

            validation.AddResult(result.Error);
            return null;
        }

        private T Invalid<T>(string field, string expected)
        {
            IsValid = false;
            validation.AddResult(ImportDataErrors.FieldInvalid(prefix + field, expected));
            return default!;
        }
    }
}
