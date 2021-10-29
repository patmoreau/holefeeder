using DrifterApps.Holefeeder.Budgeting.Application.Models;

using MediatR;

namespace DrifterApps.Holefeeder.Budgeting.Application.Categories.Queries
{
    public class GetCategoriesQuery : IRequest<CategoryViewModel[]>
    {
    }
}
