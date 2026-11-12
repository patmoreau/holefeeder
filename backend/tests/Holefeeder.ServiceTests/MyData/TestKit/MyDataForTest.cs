using Holefeeder.Service.MyData.Core;
using Holefeeder.Service.Shared.Core;

namespace Holefeeder.ServiceTests.MyData.TestKit;

internal static class MyDataForTest
{
    internal static MyDataAccount AMyDataAccount(Id? id = null) =>
        new(id ?? AnId(), AccountType.Checking, $"account-{Guid.NewGuid()}", Money.Valid(50m), new DateOnly(2020, 1, 1), "Everyday",
            Favorite: false, Inactive: false);

    internal static MyDataCategory AMyDataCategory(Id? id = null) =>
        new(id ?? AnId(), CategoryType.Expense, $"category-{Guid.NewGuid()}", "#123456", Money.Valid(20m), Favorite: false,
            System: false, Inactive: false);

    internal static MyDataCashflow AMyDataCashflow(Id accountId, Id categoryId, Id? id = null) =>
        new(id ?? AnId(), new DateOnly(2024, 1, 15), Money.Valid(1500m), IntervalType.Monthly, 1, 0, "Rent",
            TagList.Create(["home", "bills"]), categoryId, accountId, Inactive: false);

    internal static MyDataTransaction AMyDataTransaction(Id accountId, Id categoryId, Id? id = null) =>
        new(id ?? AnId(), new DateOnly(2024, 3, 1), Money.Valid(10m), "Coffee", TagList.Create(["coffee", "treat"]), categoryId,
            accountId, null, null);
}
