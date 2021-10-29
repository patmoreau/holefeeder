using System.Collections.Generic;

using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemsQuery : IRequest<QueryResult<StoreItemViewModel>>
    {
        public int? Offset { get; init; }
        
        public int? Limit { get; init; }
        
        public IEnumerable<string> Sort { get; init; }

        public IEnumerable<string> Filter { get; init; }

        public QueryParams Query => new QueryParams(Offset ?? QueryParams.DEFAULT_OFFSET,
            Limit ?? QueryParams.DEFAULT_LIMIT, Sort ?? QueryParams.DefaultSort, Filter ?? QueryParams.DefaultFilter);
    }
}
