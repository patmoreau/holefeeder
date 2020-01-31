using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public abstract class BaseOwnedRepository<TEntity, TSchema> : BaseRepository<TEntity, TSchema>, IBaseOwnedRepository<TEntity>
        where TEntity : BaseEntity, IOwnedEntity
        where TSchema : BaseSchema, IOwnedSchema
    {
        protected BaseOwnedRepository(IMongoCollection<TSchema> collection, IMapper mapper) : base(collection, mapper)
        {
        }

        public async Task<bool> IsOwnerAsync(string userId, string id) => await Collection.AsQueryable().AnyAsync(x => x.Id == id && x.UserId == userId);

        public async Task<long> CountAsync(string userId, QueryParams query) => await Collection.AsQueryable().Where(x => x.UserId == userId).Filter(query?.Filter).CountAsync();

        public async Task<IEnumerable<TEntity>> FindAsync(string userId, QueryParams queryParams)
        {
            var query = Collection.AsQueryable().Where(x => x.UserId == userId).Filter(queryParams?.Filter).Sort(queryParams?.Sort).Offset(queryParams?.Offset).Limit(queryParams?.Limit);
            return Mapper.Map<IEnumerable<TEntity>>(await query.ToListAsync());
        }
    }
}