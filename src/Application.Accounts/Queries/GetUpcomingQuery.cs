using System;
using DrifterApps.Holefeeder.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Queries
{
    public class GetUpcomingQuery : IRequest<UpcomingViewModel[]>
    {
        public Guid UserId { get; }
        public DateTime From { get; }
        public DateTime To { get; }

        public GetUpcomingQuery(Guid userId, DateTime from, DateTime to)
        {
            UserId = userId;
            From = from;
            To = to;
        }
    }
}
