using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Sync.Core;

namespace Holefeeder.ServiceTests.Sync.TestKit;

internal static class SyncRecordForTest
{
    internal static SyncAccount ASyncAccount(Id? id = null) =>
        new(id ?? AnId(), AccountType.Checking, $"account-{Guid.NewGuid()}", Money.Valid(50m), new DateOnly(2020, 1, 1), "Everyday",
            false, false);

    internal static SyncCategory ASyncCategory(Id? id = null) =>
        new(id ?? AnId(), CategoryType.Expense, $"category-{Guid.NewGuid()}", "#123456", Money.Valid(20m), false, false, false);

    internal static SyncStoreItem ASyncStoreItem(Id? id = null) =>
        new(id ?? AnId(), $"code-{Guid.NewGuid()}", "original");

    internal static SyncTransaction ASyncTransaction(Id accountId, Id categoryId, Id? id = null) =>
        new(id ?? AnId(), new DateOnly(2024, 3, 1), Money.Valid(10m), "Coffee", accountId, categoryId, null, null,
            TagList.Create(["coffee", "treat"]));

    internal static SyncCashflow ASyncCashflow(Id accountId, Id categoryId, Id? id = null) =>
        new(id ?? AnId(), new DateOnly(2024, 1, 1), IntervalType.Monthly, 1, 0, Money.Valid(45m), "Phone", accountId, categoryId, false,
            TagList.Create(["phone"]));
}
