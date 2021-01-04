using System;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Queries
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
