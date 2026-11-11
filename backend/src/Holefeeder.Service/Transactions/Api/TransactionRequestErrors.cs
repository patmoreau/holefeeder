using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Transactions.Api;

internal static class TransactionRequestErrors
{
    public const string CodeAccountIdInvalid = "account-id-invalid";
    public const string CodeCategoryIdInvalid = "category-id-invalid";

    public static ResultError AccountIdInvalid => new(CodeAccountIdInvalid, "Account id must be an id.");

    public static ResultError CategoryIdInvalid => new(CodeCategoryIdInvalid, "Category id must be an id.");
}
