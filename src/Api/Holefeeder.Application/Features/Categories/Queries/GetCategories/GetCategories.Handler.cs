using Holefeeder.Application.Context;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Categories.Queries.GetCategories;

internal class Handler : IRequestHandler<Request, QueryResult<CategoryViewModel>>
{
    private readonly IUserContext _userContext;
    private readonly BudgetingContext _context;

    public Handler(IUserContext userContext, BudgetingContext context)
    {
        _userContext = userContext;
        _context = context;
    }

    public async Task<QueryResult<CategoryViewModel>> Handle(Request request, CancellationToken cancellationToken)
    {
        var result = await _context.Categories.AsQueryable()
            .Where(x => x.UserId == _userContext.UserId)
            .ToListAsync(cancellationToken);

        return new QueryResult<CategoryViewModel>(result.Count, CategoryMapper.MapToDto(result));
    }
}
