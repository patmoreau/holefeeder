using Holefeeder.Application.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Holefeeder.Application.Extensions;

internal static class HttpContextExtensions
{
    public static ValueTask<TRequest?> ToQueryRequest<TRequest>(this HttpContext context,
        Func<int, int, string[], string[], TRequest?> createInstance) where TRequest : IRequestQuery
    {
        const string offsetKey = "offset";
        const string limitKey = "limit";
        const string sortKey = "sort";
        const string filterKey = "filter";

        bool hasOffset = int.TryParse(context.Request.Query[offsetKey], out int offset);
        bool hasLimit = int.TryParse(context.Request.Query[limitKey], out int limit);
        StringValues sort = context.Request.Query[sortKey];
        StringValues filter = context.Request.Query[filterKey];

        TRequest? result = createInstance(hasOffset ? offset : QueryParams.DEFAULT_OFFSET,
            hasLimit ? limit : QueryParams.DEFAULT_LIMIT, sort!, filter!);

        return ValueTask.FromResult(result);
    }
}
