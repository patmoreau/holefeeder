using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries;

public static class GetDateIntervalTypes
{
    public record Request : IRequest<DateIntervalType[]>;

    public class Handler : IRequestHandler<Request, DateIntervalType[]>
    {
        public Task<DateIntervalType[]> Handle(Request query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumeration.GetAll<DateIntervalType>().ToArray());
        }
    }
}
