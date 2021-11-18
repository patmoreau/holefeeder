using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries;

public static class GetAccountTypes
{
    public record Request : IRequest<AccountType[]>;

    public class Handler : IRequestHandler<Request, AccountType[]>
    {
        public Task<AccountType[]> Handle(Request query, CancellationToken cancellationToken) =>
            Task.FromResult(Enumeration.GetAll<AccountType>().ToArray());
    }
}
