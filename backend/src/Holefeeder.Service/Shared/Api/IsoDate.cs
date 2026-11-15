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

    public static ResultError DateInvalid => new(CodeDateInvalid);

    public static ResultError CashflowDateInvalid => new(CodeCashflowDateInvalid);
}
