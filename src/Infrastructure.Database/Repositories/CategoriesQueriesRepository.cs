using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Application.Transactions.Contracts;
using DrifterApps.Holefeeder.Application.Transactions.Models;
using DrifterApps.Holefeeder.Infrastructure.Database.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Infrastructure.Database.Repositories
{
    public class CategoriesQueriesRepository : RepositoryRoot, ICategoryQueries
    {
        private readonly IMapper _mapper;

        public CategoriesQueriesRepository(IMongoDbContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            var categoryCollection = await DbContext.GetCategoriesAsync(cancellationToken);

            var categories = await categoryCollection.AsQueryable()
                .Where(x => !x.System && x.UserId == "5ecdf491367f4e00016c87eb").ToListAsync(cancellationToken: cancellationToken);

            return _mapper.Map<IEnumerable<CategoryViewModel>>(categories);
        }
    }
}
