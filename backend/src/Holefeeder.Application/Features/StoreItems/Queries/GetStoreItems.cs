using System.Reflection;

using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.Application.Extensions;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.StoreItems.Queries;

public static class GetStoreItems
{
    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter) : IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal record Response(Guid Id, string Code, string Data);

    internal class Validator : QueryValidatorRoot<Request>;
}
