using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Schemas;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Repositories
{
    public class StoreRepository : IStoreRepository
    {
        private readonly IMongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;

        public IUnitOfWork UnitOfWork { get; }

        public StoreRepository(IUnitOfWork unitOfWork, IMongoDbContext mongoDbContext, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            _mongoDbContext = mongoDbContext;
            _mapper = mapper;
        }

        public async Task<StoreItem> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        {
            var collection = await _mongoDbContext.GetStoreItemsAsync(cancellationToken);

            var schema = await collection.AsQueryable().SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId,
                cancellationToken: cancellationToken);

            return _mapper.Map<StoreItem>(schema);
        }

        public async Task<StoreItem> FindByCodeAsync(Guid userId, string code,
            CancellationToken cancellationToken = default)
        {
            var collection = await _mongoDbContext.GetStoreItemsAsync(cancellationToken);

            var schema = await collection.AsQueryable().SingleOrDefaultAsync(
                x => x.Code.ToLowerInvariant() == code.ToLowerInvariant() && x.UserId == userId,
                cancellationToken: cancellationToken);

            return _mapper.Map<StoreItem>(schema);
        }

        public async Task SaveAsync(StoreItem entity, CancellationToken cancellationToken)
        {
            var collection = await _mongoDbContext.GetStoreItemsAsync(cancellationToken);

            var id = entity.Id;
            var userId = entity.UserId;

            var schema = await collection.AsQueryable().SingleOrDefaultAsync(x => x.Id == id && x.UserId == userId,
                cancellationToken: cancellationToken);

            schema = schema == null ? _mapper.Map<StoreItemSchema>(entity) : _mapper.Map(entity, schema);

            _mongoDbContext.AddCommand(async () =>
            {
                await collection.ReplaceOneAsync(x => x.Id == schema.Id,
                    schema,
                    new ReplaceOptions {IsUpsert = true},
                    cancellationToken);
            });
        }
    }
}
