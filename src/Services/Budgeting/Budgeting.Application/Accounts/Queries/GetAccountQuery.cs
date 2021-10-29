using System;

using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Accounts.Queries
{
    public class GetAccountQuery : IRequest<AccountViewModel>
    {
        public Guid Id { get; }

        public GetAccountQuery(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            Id = id;
        }
    }
}
