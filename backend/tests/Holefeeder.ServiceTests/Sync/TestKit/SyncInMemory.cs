using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.Service.Sync.Core;

namespace Holefeeder.ServiceTests.Sync.TestKit;

internal sealed class SyncInMemory : ISyncStore
{
    private Dictionary<(Id UserId, SyncTable Table, Id Id), ISyncRecord> _records = [];
    private ResultError? _failure;

    public Task<Result<ISyncSession>> BeginAsync(User user, CancellationToken cancellationToken)
    {
        if (_failure is not null)
        {
            return Task.FromResult<Result<ISyncSession>>(_failure);
        }

        return Task.FromResult<Result<ISyncSession>>(new Session(this, user, new Dictionary<(Id, SyncTable, Id), ISyncRecord>(_records)));
    }

    public void Add(User user, ISyncRecord record) => _records[(user.Id, record.Table, record.Id)] = record;

    public ISyncRecord? Find(User user, SyncTable table, Id id) => _records.GetValueOrDefault((user.Id, table, id));

    public int Count => _records.Count;

    public void FailWith(ResultError failure) => _failure = failure;

    private sealed class Session(SyncInMemory store, User user, Dictionary<(Id UserId, SyncTable Table, Id Id), ISyncRecord> working)
        : ISyncSession
    {
        public Task<Result<ISyncRecord>> FindAsync(SyncTable table, Id id, CancellationToken cancellationToken) =>
            Task.FromResult(working.TryGetValue((user.Id, table, id), out var record)
                ? record.ToResult()
                : SyncRecordErrors.NotFound(table).ToResult<ISyncRecord>());

        public Task<Result<Nothing>> SaveAsync(ISyncRecord record, CancellationToken cancellationToken)
        {
            if (working.Keys.Any(key => key.UserId != user.Id && key.Table == record.Table && key.Id == record.Id))
            {
                return Task.FromResult<Result<Nothing>>(SyncRecordErrors.NotFound(record.Table));
            }

            working[(user.Id, record.Table, record.Id)] = record;
            return Task.FromResult<Result<Nothing>>(Nothing.Value);
        }

        public Task<Result<Nothing>> DeleteAsync(SyncTable table, Id id, CancellationToken cancellationToken) =>
            Task.FromResult<Result<Nothing>>(working.Remove((user.Id, table, id)) ? Nothing.Value : SyncRecordErrors.NotFound(table));

        public Task<Result<Nothing>> CommitAsync(CancellationToken cancellationToken)
        {
            store._records = working;
            return Task.FromResult<Result<Nothing>>(Nothing.Value);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
