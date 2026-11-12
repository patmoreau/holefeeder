namespace Holefeeder.Service.MyData.Core;

internal sealed record MyDataSet(
    IReadOnlyList<MyDataAccount> Accounts,
    IReadOnlyList<MyDataCategory> Categories,
    IReadOnlyList<MyDataCashflow> Cashflows,
    IReadOnlyList<MyDataTransaction> Transactions);
