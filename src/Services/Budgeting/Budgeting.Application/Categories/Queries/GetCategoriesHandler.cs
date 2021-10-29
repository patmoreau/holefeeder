using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Categories.Queries
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, CategoryViewModel[]>
    {
        private readonly ICategoryQueriesRepository _categoryQueriesRepository;
        private readonly ItemsCache _cache;

        public GetCategoriesHandler(ICategoryQueriesRepository categoryQueriesRepository, ItemsCache cache)
        {
            _categoryQueriesRepository = categoryQueriesRepository;
            _cache = cache;
        }

        public Task<CategoryViewModel[]> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return HandleInternal(cancellationToken);
        }

        private async Task<CategoryViewModel[]> HandleInternal(CancellationToken cancellationToken)
        {
            return (await _categoryQueriesRepository.GetCategoriesAsync((Guid)_cache["UserId"], cancellationToken)).ToArray();
        }
    }
}
