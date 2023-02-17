using Holefeeder.Application.Context;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Categories.Queries;

public class GetCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/categories",
                async (IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) = await mediator.Send(new Request(), cancellationToken);
                    ctx.Response.Headers.Add("X-Total-Count", $"{total}");
                    return Results.Ok(viewModels);
                })
            .Produces<IEnumerable<CategoryViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags(nameof(Categories))
            .WithName(nameof(GetCategories))
            .RequireAuthorization();
    }

    internal class Validator : AbstractValidator<Request>
    {
    }

    internal record Request : IRequest<QueryResult<CategoryViewModel>>;

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
            var result = await _context.Categories
                .Where(x => x.UserId == _userContext.UserId)
                .OrderByDescending(x => x.Favorite)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return new QueryResult<CategoryViewModel>(result.Count, CategoryMapper.MapToDto(result));
        }
    }
}
