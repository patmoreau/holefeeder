using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public record GetStoreItemsQuery(int Offset, int Limit, string[] Sort, string[] Filter)
        : IRequest<(int Total, IEnumerable<StoreItemViewModel> Items)>, IQuery
    {
        public static ValueTask<GetStoreItemsQuery?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string offsetKey = "offset";
            const string limitKey = "limit";
            const string sortKey = "sort";
            const string filterKey = "filter";

            var hasOffset = int.TryParse(context.Request.Query[offsetKey], out var offset);
            var hasLimit = int.TryParse(context.Request.Query[limitKey], out var limit);
            var sort = context.Request.Query[sortKey].ToArray();
            var filter = context.Request.Query[filterKey].ToArray();

            var result = new GetStoreItemsQuery(hasOffset ? offset : QueryParams.DEFAULT_OFFSET,
                hasLimit ? limit : QueryParams.DEFAULT_LIMIT, sort, filter);

            return ValueTask.FromResult<GetStoreItemsQuery?>(result);
        }
    }
}
