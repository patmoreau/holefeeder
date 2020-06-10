using System;
using System.Collections.Generic;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Application.SeedWork;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetAccountsQuery : IRequest<AccountViewModel[]>
    {
        public Guid UserId { get; }
        public QueryParams Query { get; }

        public GetAccountsQuery(Guid userId, int offset, int limit, IEnumerable<string> sort, IEnumerable<string> filter)
        {
            UserId = userId;
            Query = new QueryParams(offset, limit, sort, filter);
        }
    }
}
