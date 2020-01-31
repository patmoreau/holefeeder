using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public class ObjectsRepository : BaseOwnedRepository<ObjectDataEntity, ObjectDataSchema>, IObjectsRepository
    {
        const string INDEX_UNIQUE_NAME = "userId_code_unique_index";
        private readonly IMongoCollection<ObjectDataSchema> _objects;

        public ObjectsRepository(IMongoCollection<ObjectDataSchema> collection, IMapper mapper) : base(collection, mapper)
        {
            _objects = collection.ThrowIfNull(nameof(collection));
        }

        public async Task<ObjectDataEntity> FindByCodeAsync(string userId, string code) => Mapper.Map<ObjectDataEntity>(await _objects.AsQueryable().Where(x => x.UserId == userId && x.Code == code).FirstOrDefaultAsync());
    }
}