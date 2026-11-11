using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Accounts.Api;

internal static class OpenDate
{
    public static Result<DateOnly> Parse(string? value)
    {
        var date = IsoDate.Parse(value, OpenDateErrors.Invalid);
        return date.IsSuccess && date.Value == DateOnly.MinValue ? OpenDateErrors.Invalid : date;
    }
}

internal static class OpenDateErrors
{
    public const string CodeInvalid = "open-date-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Open date must be a date as YYYY-MM-DD.");
}
