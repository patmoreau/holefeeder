using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries;

public static class GetCategoryTypes
{
    public record Request : IRequest<CategoryType[]>;

    public class Handler : IRequestHandler<Request, CategoryType[]>
    {
        public Task<CategoryType[]> Handle(Request query, CancellationToken cancellationToken)
        {
            return Task.FromResult(Enumeration.GetAll<CategoryType>().ToArray());
        }
    }
}
