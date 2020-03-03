using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class ObjectsRepository : BaseOwnedRepository<ObjectDataEntity, ObjectDataSchema>, IObjectsRepository
    {
        private readonly IMongoCollection<ObjectDataSchema> _objects;

        public ObjectsRepository(IMongoCollection<ObjectDataSchema> collection, IMapper mapper) : base(collection, mapper)
        {
            _objects = collection.ThrowIfNull(nameof(collection));
        }

        public async Task<ObjectDataEntity> FindByCodeAsync(string userId, string code, CancellationToken cancellationToken = default) =>
            Mapper.Map<ObjectDataEntity>(await _objects.AsQueryable().Where(x => x.UserId.Equals(userId) && x.Code.Equals(code)).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false));
    }
}