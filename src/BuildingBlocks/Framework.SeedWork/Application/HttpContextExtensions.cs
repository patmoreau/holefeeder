using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace DrifterApps.Holefeeder.Framework.SeedWork.Application;

public static class HttpContextExtensions
{
    public static ValueTask<TRequest?> ToQueryRequest<TRequest>(this HttpContext context,
        Func<int, int, string[], string[], TRequest?> createInstance) where TRequest : IRequestQuery
    {
        const string offsetKey = "offset";
        const string limitKey = "limit";
        const string sortKey = "sort";
        const string filterKey = "filter";

        var hasOffset = int.TryParse(context.Request.Query[offsetKey], out var offset);
        var hasLimit = int.TryParse(context.Request.Query[limitKey], out var limit);
        var sort = context.Request.Query[sortKey].ToArray();
        var filter = context.Request.Query[filterKey].ToArray();

        var result = createInstance(hasOffset ? offset : QueryParams.DEFAULT_OFFSET,
            hasLimit ? limit : QueryParams.DEFAULT_LIMIT, sort, filter);

        return ValueTask.FromResult(result);
    }
}
