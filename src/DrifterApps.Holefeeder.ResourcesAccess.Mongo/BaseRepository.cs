using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using MongoDB.Bson;
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

        public Task<int> CountAsync(QueryParams query, CancellationToken cancellationToken = default) =>
            Collection.AsQueryable().Filter(query?.Filter).CountAsync(cancellationToken);

        public async Task<IEnumerable<TEntity>> FindAsync(QueryParams queryParams,
            CancellationToken cancellationToken = default)
        {
            var query = Collection.AsQueryable().Filter(queryParams?.Filter).Sort(queryParams?.Sort)
                .Offset(queryParams?.Offset).Limit(queryParams?.Limit);
            return Mapper.Map<IEnumerable<TEntity>>(await query.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        public async Task<TEntity> FindByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (ObjectId.TryParse(id, out _))
            {
                return Mapper.Map<TEntity>(await Collection.Find(x => x.Id == id)
                    .SingleOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false));
            }

            var globalId = Guid.Parse(id);
            return Mapper.Map<TEntity>(await Collection.Find(x => x.GlobalId == globalId)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false));
        }

        public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var schema = Mapper.Map<TSchema>(entity);
            if (schema.GlobalId == Guid.Empty)
            {
                schema.GlobalId = Guid.NewGuid();
            }

            await Collection
                .InsertOneAsync(schema, new InsertOneOptions {BypassDocumentValidation = false}, cancellationToken)
                .ConfigureAwait(false);
            return Mapper.Map<TEntity>(schema);
        }

        public async Task UpdateAsync(string id, TEntity entity, CancellationToken cancellationToken = default)
        {
            if (ObjectId.TryParse(id, out _))
            {
                var schema = await Collection.Find(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);

                schema = Mapper.Map(entity, schema);
                schema.Id = id;
                await Collection.ReplaceOneAsync(m => m.Id == id, schema, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                var globalId = Guid.Parse(id);
                var schema = await Collection.Find(x => x.GlobalId == globalId).SingleOrDefaultAsync(cancellationToken);

                schema = Mapper.Map(entity, schema);
                schema.GlobalId = globalId;
                await Collection.ReplaceOneAsync(m => m.GlobalId == globalId, schema,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        public Task RemoveAsync(string id, CancellationToken cancellationToken = default)
        {
            if (ObjectId.TryParse(id, out _))
            {
                return Collection.DeleteOneAsync(m => m.Id == id, cancellationToken);
            }
            else
            {
                var globalId = Guid.Parse(id);
                return Collection.DeleteOneAsync(m => m.GlobalId == globalId, cancellationToken);
            }
        }
    }
}
