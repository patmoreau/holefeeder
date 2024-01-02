using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Categories.Queries;

public class GetCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/categories",
                async (IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) =
                        await mediator.Send(new Request(), cancellationToken);
                    ctx.Response.Headers.Append("X-Total-Count", $"{total}");
                    return Results.Ok(viewModels);
                })
            .Produces<IEnumerable<CategoryViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithTags(nameof(Categories))
            .WithName(nameof(GetCategories))
            .RequireAuthorization();

    internal class Validator : AbstractValidator<Request>;

    internal record Request : IRequest<QueryResult<CategoryViewModel>>;

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, QueryResult<CategoryViewModel>>
    {
        private readonly BudgetingContext _context = context;
        private readonly IUserContext _userContext = userContext;

        public async Task<QueryResult<CategoryViewModel>> Handle(Request request, CancellationToken cancellationToken)
        {
            List<Category> result = await _context.Categories
                .Where(x => x.UserId == _userContext.Id)
                .OrderByDescending(x => x.Favorite)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);

            return new QueryResult<CategoryViewModel>(result.Count, CategoryMapper.MapToDto(result));
        }
    }
}
