using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core;

namespace Holefeeder.ServiceTests.Transactions.TestKit;

internal static class TransactionsForTest
{
    public static TransactionAccount AnAccount(string name = "Chequing", bool favorite = false) =>
        new(AnId(), name, favorite);

    public static TransactionCategory ACategory(
        string name = "Groceries", CategoryType type = CategoryType.Expense, bool favorite = false, string color = "#34C759") =>
        new(AnId(), name, color, type, favorite);
}
