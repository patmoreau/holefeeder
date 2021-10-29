using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries
{
    public class GetAccountsQuery : IRequest<QueryResult<AccountViewModel>>
    {
        public QueryParams Query { get; }

        public GetAccountsQuery(int? offset, int? limit, IEnumerable<string> sort,
            IEnumerable<string> filter)
        {
            Query = new QueryParams(offset ?? QueryParams.DEFAULT_OFFSET, limit ?? QueryParams.DEFAULT_LIMIT,
                sort ?? QueryParams.DefaultSort, filter ?? QueryParams.DefaultFilter);
        }
    }
}
