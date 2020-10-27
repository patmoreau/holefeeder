using DrifterApps.Holefeeder.Application.Transactions.Models;
using MediatR;

namespace DrifterApps.Holefeeder.Application.Transactions.Queries
{
    public class GetCategoriesQuery : IRequest<CategoryViewModel[]>
    {
    }
}
