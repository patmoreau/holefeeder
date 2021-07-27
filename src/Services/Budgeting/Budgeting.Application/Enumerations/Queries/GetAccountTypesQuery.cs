using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Enumerations.Queries
{
    public class GetAccountTypesQuery : IRequest<AccountType[]>
    {
        public GetAccountTypesQuery()
        {
        }
    }
}
