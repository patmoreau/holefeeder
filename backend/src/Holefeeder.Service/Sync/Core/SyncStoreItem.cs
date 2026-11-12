using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;

namespace Holefeeder.Service.Sync.Core;

internal sealed record SyncStoreItem(Id Id, string Code, string Data) : ISyncRecord
{
    public SyncTable Table => SyncTable.StoreItems;

    public Result<ISyncRecord> Validated() =>
        SyncRecordErrors.Validated(this, (!string.IsNullOrWhiteSpace(Code), SyncRecordErrors.StoreItemCodeRequired));
}

internal sealed record SyncStoreItemPatch(string? Code = null, string? Data = null) : ISyncPatch
{
    public SyncTable Table => SyncTable.StoreItems;

    public Result<ISyncRecord> Create(Id id) =>
        new SyncStoreItem(id, Code ?? string.Empty, Data ?? string.Empty).Validated();

    public Result<ISyncRecord> ApplyTo(ISyncRecord existing)
    {
        var item = (SyncStoreItem)existing;
        return (item with { Code = Code ?? item.Code, Data = Data ?? item.Data }).Validated();
    }

    public ISyncRecord? KeptOnDelete(ISyncRecord existing) => null;
}
