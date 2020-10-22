using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Queries;
using DrifterApps.Holefeeder.Application.SeedWork;
using DrifterApps.Holefeeder.Infrastructure.Database.Schemas;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Extensions
{
    internal static class CollectionExtensions
    {
        internal static Task<int> CountAsync<T>(this IMongoCollection<T> collection, QueryParams query,
            CancellationToken cancellationToken = default)
        {
            return collection.AsQueryable().Filter(query.Filter).CountAsync(cancellationToken);
        }

        internal static async Task<IEnumerable<TSchema>> FindAsync<TSchema>(this IMongoCollection<TSchema> collection,
            QueryParams queryParams, CancellationToken cancellationToken = default) where TSchema : BaseSchema
        {
            var query = collection.AsQueryable().Filter(queryParams.Filter).Sort(queryParams.Sort)
                .Offset(queryParams.Offset).Limit(queryParams.Limit);
            return await query.ToListAsync(cancellationToken);
        }

        internal static async Task<TSchema> FindByIdAsync<TSchema>(this IMongoCollection<TSchema> collection, Guid id,
            CancellationToken cancellationToken = default) where TSchema : BaseSchema =>
            await collection.Find(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);
    }
}
