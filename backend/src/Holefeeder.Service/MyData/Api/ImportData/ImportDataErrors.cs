using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.MyData.Api.ImportData;

internal static class ImportDataErrors
{
    public const string CodeDataEmpty = "my-data-import-empty";
    public const string CodeFieldInvalid = "my-data-field-invalid";

    public static ResultError DataEmpty =>
        new(CodeDataEmpty, "Data must contain at least one account, category, cashflow or transaction.");

    public static ResultError FieldInvalid(string field, string expected) => new(CodeFieldInvalid, $"{field} must be {expected}.");
}
