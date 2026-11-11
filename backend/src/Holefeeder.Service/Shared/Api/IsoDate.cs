using System.Globalization;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Api;

internal static class IsoDate
{
    public static Result<DateOnly> Parse(string? value, ResultError error) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : error;
}

internal static class IsoDateErrors
{
    public const string CodeDateInvalid = "date-invalid";
    public const string CodeCashflowDateInvalid = "cashflow-date-invalid";

    public static ResultError DateInvalid => new(CodeDateInvalid, "Date must be a date as YYYY-MM-DD.");

    public static ResultError CashflowDateInvalid => new(CodeCashflowDateInvalid, "Cashflow date must be a date as YYYY-MM-DD.");
}
