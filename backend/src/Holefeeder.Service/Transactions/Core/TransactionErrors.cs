using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Transactions.Core;

internal static class TransactionErrors
{
    public const string CodeAccountNotFound = "account-not-found";
    public const string CodeCategoryNotFound = "category-not-found";

    public static NotFoundError AccountNotFound => new(CodeAccountNotFound, "Account was not found.");

    public static NotFoundError CategoryNotFound => new(CodeCategoryNotFound, "Category was not found.");
}
