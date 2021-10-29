using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries
{
    public class GetAccountTypesHandler : IRequestHandler<GetAccountTypesQuery, AccountType[]>
    {
        public GetAccountTypesHandler()
        {
        }

        public Task<AccountType[]> Handle(GetAccountTypesQuery query, CancellationToken cancellationToken)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return Task.FromResult(Enumeration.GetAll<AccountType>().ToArray());
        }
    }
}
