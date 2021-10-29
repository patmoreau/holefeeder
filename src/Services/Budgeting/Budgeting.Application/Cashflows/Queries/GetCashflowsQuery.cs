using System.Collections.Generic;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries
{
    public class GetCashflowsQuery : IRequest<QueryResult<CashflowViewModel>>
    {
        public QueryParams Query { get; }

        public GetCashflowsQuery(int? offset, int? limit, IEnumerable<string> sort,
            IEnumerable<string> filter)
        {
            Query = new QueryParams(offset ?? QueryParams.DEFAULT_OFFSET, limit ?? QueryParams.DEFAULT_LIMIT,
                sort ?? QueryParams.DefaultSort, filter ?? QueryParams.DefaultFilter);
        }
    }
}
