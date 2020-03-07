using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Extensions;
using DrifterApps.Holefeeder.ResourcesAccess.Mongo.Schemas;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Threading;

namespace DrifterApps.Holefeeder.ResourcesAccess.Mongo
{
    public abstract class BaseOwnedRepository<TEntity, TSchema> : BaseRepository<TEntity, TSchema>, IBaseOwnedRepository<TEntity>
        where TEntity : BaseEntity, IOwnedEntity
        where TSchema : BaseSchema, IOwnedSchema
    {
        protected BaseOwnedRepository(IMongoCollection<TSchema> collection, IMapper mapper) : base(collection, mapper)
        {
        }

        public Task<bool> IsOwnerAsync(string userId, string id, CancellationToken cancellationToken = default) => Collection.AsQueryable().AnyAsync(x => x.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase) && x.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase), cancellationToken);

        public Task<int> CountAsync(string userId, QueryParams query, CancellationToken cancellationToken = default) => Collection.AsQueryable().Where(x => x.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase)).Filter(query?.Filter).CountAsync(cancellationToken);

        public async Task<IEnumerable<TEntity>> FindAsync(string userId, QueryParams queryParams, CancellationToken cancellationToken = default)
        {
            var query = Collection.AsQueryable().Where(x => x.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase)).Filter(queryParams?.Filter).Sort(queryParams?.Sort).Offset(queryParams?.Offset).Limit(queryParams?.Limit);
            return Mapper.Map<IEnumerable<TEntity>>(await query.ToListAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}