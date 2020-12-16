using System;
using System.Collections.Generic;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
{
    public class GetAccountsQuery : IRequest<AccountViewModel[]>
    {
        public Guid UserId { get; }
        public QueryParams Query { get; }

        public GetAccountsQuery(Guid userId, int? offset, int? limit, IEnumerable<string> sort, IEnumerable<string> filter)
        {
            UserId = userId;
            Query = new QueryParams(offset ?? QueryParams.DefaultOffset, limit ?? QueryParams.DefaultLimit, sort ?? QueryParams.DefaultSort, filter??QueryParams.DefaultFilter);
        }
    }
}
