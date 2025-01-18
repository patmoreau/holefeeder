namespace Holefeeder.FunctionalTests;

internal static class ContextConstants
{
    internal static class AccountContext
    {
        internal const string ExistingAccount = $"{nameof(AccountContext)}_{nameof(ExistingAccount)}";
        internal const string ExistingAccounts = $"{nameof(AccountContext)}_{nameof(ExistingAccounts)}";
    }

    internal static class CashflowContext
    {
        internal const string ExistingCashflow = $"{nameof(CashflowContext)}_{nameof(ExistingCashflow)}";
        internal const string ExistingCashflows = $"{nameof(CashflowContext)}_{nameof(ExistingCashflows)}";
    }

    internal static class CategoryContext
    {
        internal const string ExistingCategory = $"{nameof(CategoryContext)}_{nameof(ExistingCategory)}";
        internal const string ExistingCategories = $"{nameof(CategoryContext)}_{nameof(ExistingCategories)}";
    }

    internal static class StoreItemContext
    {
        internal const string ExistingStoreItem = $"{nameof(StoreItemContext)}_{nameof(ExistingStoreItem)}";
        internal const string ExistingStoreItems = $"{nameof(StoreItemContext)}_{nameof(ExistingStoreItems)}";
    }

    internal static class TransactionContext
    {
        internal const string ExistingTransaction = $"{nameof(TransactionContext)}_{nameof(ExistingTransaction)}";
        internal const string ExistingTransactions = $"{nameof(TransactionContext)}_{nameof(ExistingTransactions)}";
    }

    internal static class RequestContext
    {
        internal const string CurrentRequest = $"{nameof(RequestContext)}_{nameof(CurrentRequest)}";
    }
}
