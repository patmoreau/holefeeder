using System;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Cashflows.Queries
{
    public class GetUpcomingQuery : IRequest<UpcomingViewModel[]>
    {
        public DateTime From { get; }
        public DateTime To { get; }

        public GetUpcomingQuery(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
    }
}
