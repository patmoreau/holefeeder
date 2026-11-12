using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;

namespace Holefeeder.Service.Sync.Core.SyncData;

internal sealed class SyncDataUseCase(ISyncStore store)
{
    public async Task<Result<Nothing>> HandleAsync(SyncDataRequest request, CancellationToken cancellationToken)
    {
        if (request.User.Id == Id.Empty)
        {
            return UserErrors.NotRegistered;
        }
        if (request.Operations.Count == 0)
        {
            return Nothing.Value;
        }

        var sessionResult = await store.BeginAsync(request.User, cancellationToken);
        if (sessionResult.IsFailure)
        {
            return sessionResult.Error;
        }

        await using var session = sessionResult.Value;
        foreach (var operation in request.Operations)
        {
            var applied = await ApplyAsync(session, operation, cancellationToken);
            if (applied.IsFailure)
            {
                return applied.Error;
            }
        }

        return await session.CommitAsync(cancellationToken);
    }

    private static async Task<Result<Nothing>> ApplyAsync(ISyncSession session, SyncOperation operation, CancellationToken cancellationToken)
    {
        var patch = operation.Patch;
        var existing = await session.FindAsync(patch.Table, operation.Id, cancellationToken);
        if (existing.IsFailure && (existing.Error is not NotFoundError || operation.Kind != SyncOperationKind.Put))
        {
            return existing.Error;
        }

        if (operation.Kind == SyncOperationKind.Delete)
        {
            return patch.KeptOnDelete(existing.Value) is { } kept
                ? await session.SaveAsync(kept, cancellationToken)
                : await session.DeleteAsync(patch.Table, operation.Id, cancellationToken);
        }

        var changed = existing.IsSuccess ? patch.ApplyTo(existing.Value) : patch.Create(operation.Id);
        if (changed.IsFailure)
        {
            return changed.Error;
        }

        return await session.SaveAsync(changed.Value, cancellationToken);
    }
}
