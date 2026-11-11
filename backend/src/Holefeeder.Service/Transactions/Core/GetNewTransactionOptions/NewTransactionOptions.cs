namespace Holefeeder.Service.Transactions.Core.GetNewTransactionOptions;

internal sealed record NewTransactionOptions(IReadOnlyList<TransactionAccount> Accounts, IReadOnlyList<TransactionCategory> Categories);
