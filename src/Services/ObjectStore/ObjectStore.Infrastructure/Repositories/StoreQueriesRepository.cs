using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Framework.Mongo.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Contracts;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using DrifterApps.Holefeeder.ObjectStore.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.ObjectStore.Infrastructure.Repositories
{
    public class StoreQueriesRepository : IStoreQueriesRepository
    {
        private readonly IMongoDbContext _context;
        private readonly IMapper _mapper;

        public StoreQueriesRepository(IMongoDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StoreItemViewModel>> GetItemsAsync(Guid userId, QueryParams query,
            CancellationToken cancellationToken)
        {
            query.ThrowIfNull(nameof(query));

            var collection = await _context.GetStoreItemsAsync(cancellationToken);

            var items = (await collection
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .Filter(query.Filter)
                .Sort(query.Sort)
                .Offset(query.Offset)
                .Limit(query.Limit)
                .ToListAsync(cancellationToken));

            return _mapper.Map<IEnumerable<StoreItemViewModel>>(items);
        }

        public async Task<StoreItemViewModel> GetItemAsync(Guid userId, Guid id,
            CancellationToken cancellationToken)
        {
            id.ThrowIfNull(nameof(id));

            var collection = await _context.GetStoreItemsAsync(cancellationToken);

            var item = await collection
                .AsQueryable()
                .SingleOrDefaultAsync(t => t.UserId == userId && t.Id == id, cancellationToken: cancellationToken);

            return _mapper.Map<StoreItemViewModel>(item);
        }
    }
}
