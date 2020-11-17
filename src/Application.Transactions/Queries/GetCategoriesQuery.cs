using System;
using DrifterApps.Holefeeder.Application.Transactions.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Transactions.Queries
{
    public class GetCategoriesQuery : IRequest<CategoryViewModel[]>
    {
        public Guid UserId { get; }

        public GetCategoriesQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
