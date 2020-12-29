using System;
using System.Collections.Generic;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.ObjectStore.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Queries
{
    public class GetStoreItemsQuery : IRequest<StoreItemViewModel[]>
    {
        public Guid UserId { get; }

        public QueryParams Query { get; }

        public GetStoreItemsQuery(Guid userId, int? offset, int? limit, IEnumerable<string> sort,
            IEnumerable<string> filter)
        {
            UserId = userId;
            Query = new QueryParams(offset ?? QueryParams.DefaultOffset, limit ?? QueryParams.DefaultLimit,
                sort ?? QueryParams.DefaultSort, filter ?? QueryParams.DefaultFilter);
        }
    }
}
