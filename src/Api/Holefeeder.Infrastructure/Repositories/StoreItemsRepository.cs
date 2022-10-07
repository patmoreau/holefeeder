using Dapper;

using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class StoreItemsRepository : IStoreItemsRepository
{
    private const string SELECT_CODE =
        "SELECT id, code, data, user_id FROM store_items WHERE lower(code) = lower(@Code) AND user_id = @UserId;";

    private readonly IObjectStoreContext _context;

    public StoreItemsRepository(IObjectStoreContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

    public async Task<StoreItem?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection.FindByIdAsync<StoreItemEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false);

        return StoreItemMapper.MapToModelOrNull(schema);
    }

    public async Task<StoreItem?> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection
            .QuerySingleOrDefaultAsync<StoreItemEntity>(SELECT_CODE, new {Code = code, UserId = userId})
            .ConfigureAwait(false);

        return StoreItemMapper.MapToModelOrNull(schema);
    }

    public async Task SaveAsync(StoreItem model, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = model.Id;
        var userId = model.UserId;

        var entity = await transaction.FindByIdAsync<StoreItemEntity>(new {Id = id, UserId = userId})
            .ConfigureAwait(false);

        if (entity is null)
        {
            await transaction.InsertAsync(StoreItemMapper.MapToEntity(model)).ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(StoreItemMapper.MapToEntity(model)).ConfigureAwait(false);
        }
    }
}
