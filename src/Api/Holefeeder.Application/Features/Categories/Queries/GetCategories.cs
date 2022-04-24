﻿using Carter;

using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Categories.Queries;

public class GetCategories : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/categories",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) = await mediator.Send(request, cancellationToken);
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

    public record Request : IRequest<QueryResult<CategoryViewModel>>;

    public class Handler : IRequestHandler<Request, QueryResult<CategoryViewModel>>
    {
        private readonly IUserContext _userContext;
        private readonly ICategoryQueriesRepository _categoryQueriesRepository;

        public Handler(IUserContext userContext, ICategoryQueriesRepository categoryQueriesRepository)
        {
            _userContext = userContext;
            _categoryQueriesRepository = categoryQueriesRepository;
        }

        public async Task<QueryResult<CategoryViewModel>> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = (await _categoryQueriesRepository.GetCategoriesAsync(_userContext.UserId, cancellationToken))
                .ToList();

            return new QueryResult<CategoryViewModel>(result.Count, result);
        }
    }
}
