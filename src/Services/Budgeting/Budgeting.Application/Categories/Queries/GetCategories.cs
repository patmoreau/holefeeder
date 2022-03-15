using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Categories.Queries;

public static class GetCategories
{
    public record Request : IRequest<ListRequestResult>;

    public class Handler : IRequestHandler<Request, ListRequestResult>
    {
        private readonly ItemsCache _cache;
        private readonly ICategoryQueriesRepository _categoryQueriesRepository;

        public Handler(ICategoryQueriesRepository categoryQueriesRepository, ItemsCache cache)
        {
            _categoryQueriesRepository = categoryQueriesRepository;
            _cache = cache;
        }

        public async Task<ListRequestResult> Handle(Request request, CancellationToken cancellationToken)
        {
            var result =
                (await _categoryQueriesRepository.GetCategoriesAsync((Guid)_cache["UserId"], cancellationToken))
                .ToList();

            return new ListRequestResult(result.Count, result);
        }
    }
}
