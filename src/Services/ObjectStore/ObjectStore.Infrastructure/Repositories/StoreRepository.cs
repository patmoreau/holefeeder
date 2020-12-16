using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;
using DrifterApps.Holefeeder.ObjectStore.Domain.BoundedContext.StoreItemContext;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Schemas;
using MongoDB.Driver;

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

        public async Task CreateAsync(StoreItem entity, CancellationToken cancellationToken)
        {
            var schema = _mapper.Map<StoreItemSchema>(entity);

            var collection = await _mongoDbContext.GetStoreItemsAsync(cancellationToken);

            _mongoDbContext.AddCommand(async () =>
            {
                await collection.InsertOneAsync(schema, new InsertOneOptions {BypassDocumentValidation = false},
                    cancellationToken);
            });
        }
    }
}
