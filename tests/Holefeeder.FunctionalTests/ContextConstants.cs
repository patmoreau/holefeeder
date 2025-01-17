namespace Holefeeder.FunctionalTests;

internal static class ContextConstants
{
    internal static class AccountContexts
    {
        internal const string ExistingAccount = $"{nameof(AccountContexts)}_{nameof(ExistingAccount)}";
        internal const string ExistingAccounts = $"{nameof(AccountContexts)}_{nameof(ExistingAccounts)}";
    }

    internal static class CashflowContexts
    {
        internal const string CashflowRequest = $"{nameof(CashflowContexts)}_{nameof(CashflowRequest)}";
        internal const string ExistingCashflow = $"{nameof(CashflowContexts)}_{nameof(ExistingCashflow)}";
        internal const string ExistingCashflows = $"{nameof(CashflowContexts)}_{nameof(ExistingCashflows)}";
    }

    internal static class CategoryContexts
    {
        internal const string ExistingCategory = $"{nameof(CategoryContexts)}_{nameof(ExistingCategory)}";
        internal const string ExistingCategories = $"{nameof(CategoryContexts)}_{nameof(ExistingCategories)}";
    }

    internal static class StoreItemContext
    {
        internal const string ExistingStoreItem = $"{nameof(StoreItemContext)}_{nameof(ExistingStoreItem)}";
        internal const string ExistingStoreItems = $"{nameof(StoreItemContext)}_{nameof(ExistingStoreItems)}";
    }

    internal static class TransactionContexts
    {
        internal const string ExistingTransaction = $"{nameof(TransactionContexts)}_{nameof(ExistingTransaction)}";
        internal const string ExistingTransactions = $"{nameof(TransactionContexts)}_{nameof(ExistingTransactions)}";
    }

    internal static class RequestContext
    {
        internal const string CurrentRequest = $"{nameof(RequestContext)}_{nameof(CurrentRequest)}";
    }
}
