using System;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Entities;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Mapping;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Repositories;

public class StoreItemsRepository : IStoreItemsRepository
{
    private readonly IObjectStoreContext _context;
    private readonly StoreItemMapper _storeItemMapper;

    public IUnitOfWork UnitOfWork => _context;

    public StoreItemsRepository(IObjectStoreContext context, StoreItemMapper storeItemMapper)
    {
        _context = context;
        _storeItemMapper = storeItemMapper;
    }

    public async Task<StoreItem?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection.FindByIdAsync<StoreItemEntity>(new { Id = id, UserId = userId })
            .ConfigureAwait(false);

        return _storeItemMapper.MapToModelOrNull(schema);
    }

    public async Task<StoreItem?> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection
            .QuerySingleOrDefaultAsync<StoreItemEntity>(SELECT_CODE, new { Code = code, UserId = userId })
            .ConfigureAwait(false);

        return _storeItemMapper.MapToModelOrNull(schema);
    }

    public async Task SaveAsync(StoreItem model, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = model.Id;
        var userId = model.UserId;

        var entity = await transaction.FindByIdAsync<StoreItemEntity>(new { Id = id, UserId = userId })
            .ConfigureAwait(false);

        if (entity is null)
        {
            await transaction.InsertAsync(_storeItemMapper.MapToEntity(model)).ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(_storeItemMapper.MapToEntity(model)).ConfigureAwait(false);
        }
    }

    private const string SELECT_CODE =
        "SELECT id, code, data, user_id FROM store_items WHERE lower(code) = lower(@Code) AND user_id = @UserId;";
}
