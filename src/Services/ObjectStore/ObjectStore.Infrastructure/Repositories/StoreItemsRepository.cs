using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Dapper;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Entities;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Repositories
{
    public class StoreItemsRepository : IStoreItemsRepository
    {
        private readonly IObjectStoreContext _context;
        private readonly IMapper _mapper;

        public IUnitOfWork UnitOfWork => _context;

        public StoreItemsRepository(IObjectStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<StoreItem> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
        {
            var connection = _context.Connection;

            var schema = await connection.FindByIdAsync<StoreItemEntity>(new { Id = id, UserId = userId })
                .ConfigureAwait(false);

            return _mapper.Map<StoreItem>(schema);
        }

        public async Task<StoreItem> FindByCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
        {
            var connection = _context.Connection;

            var schema = await connection
                .QuerySingleOrDefaultAsync<StoreItemEntity>(SELECT_CODE, new { Code = code, UserId = userId })
                .ConfigureAwait(false);

            return _mapper.Map<StoreItem>(schema);
        }

        public async Task SaveAsync(StoreItem entity, CancellationToken cancellationToken)
        {
            var transaction = _context.Transaction;

            var id = entity.Id;
            var userId = entity.UserId;

            var record = await transaction.FindByIdAsync<StoreItemEntity>(new { Id = id, UserId = userId })
                .ConfigureAwait(false);

            if (record is null)
            {
                await transaction.InsertAsync(_mapper.Map<StoreItemEntity>(entity)).ConfigureAwait(false);
            }
            else
            {
                await transaction.UpdateAsync(_mapper.Map(entity, record)).ConfigureAwait(false);
            }
        }

        private const string SELECT_CODE =
            "SELECT id, code, data, user_id FROM store_items WHERE lower(code) = lower(@Code) AND user_id = @UserId;";
    }
}
