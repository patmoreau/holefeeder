using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal static class TransactionErrors
{
    public const string CodeNotFound = "transaction-not-found";
    public const string CodeAccountMissing = "transaction-account-missing";
    public const string CodeCategoryMissing = "transaction-category-missing";
    public const string CodeCategoryNotFound = "category-not-found";
    public const string CodeTransferCategoriesNotFound = "transfer-categories-not-found";
    public const string CodeSameAccount = "transfer-same-account";

    public static NotFoundError NotFound => new(CodeNotFound);

    public static NotFoundError CategoryNotFound => new(CodeCategoryNotFound);

    public static NotFoundError TransferCategoriesNotFound =>
        new(CodeTransferCategoriesNotFound);

    public static ResultError AccountMissing(Id id) => new(CodeAccountMissing, $"Account '{id.Value}' does not exists.");

    public static ResultError CategoryMissing(Id id) => new(CodeCategoryMissing, $"Category '{id.Value}' does not exists.");

    public static ResultError SameAccount => new(CodeSameAccount);
}
