using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public abstract class BaseRepository<TEntity, TSchema> where TSchema : BaseSchema
    {
        protected IMongoCollection<TSchema> Collection { get; }
        protected IMapper Mapper { get; }

        protected BaseRepository(IMongoCollection<TSchema> collection, IMapper mapper)
        {
            Collection = collection.ThrowIfNull(nameof(collection));
            Mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public async Task<long> CountAsync(QueryParams query) => await Collection.AsQueryable().Filter(query?.Filter).CountAsync();

        public async Task<IEnumerable<TEntity>> FindAsync(QueryParams queryParams)
        {
            var query = Collection.AsQueryable().Filter(queryParams?.Filter).Sort(queryParams?.Sort).Offset(queryParams?.Offset).Limit(queryParams?.Limit);
            return Mapper.Map<IEnumerable<TEntity>>(await query.ToListAsync());
        }

        public async Task<TEntity> FindByIdAsync(string id) => Mapper.Map<TEntity>(await Collection.Find(x => x.Id == id).SingleOrDefaultAsync());

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            var schema = Mapper.Map<TSchema>(entity);
            await Collection.InsertOneAsync(schema);
            return Mapper.Map<TEntity>(schema);
        }

        public async Task UpdateAsync(string id, TEntity entity)
        {
            var schema = Mapper.Map<TSchema>(entity);
            schema.Id = id;
            await Collection.ReplaceOneAsync(m => m.Id == id, schema);
        }

        public async Task RemoveAsync(string id) => await Collection.DeleteOneAsync(m => m.Id == id);
    }
}