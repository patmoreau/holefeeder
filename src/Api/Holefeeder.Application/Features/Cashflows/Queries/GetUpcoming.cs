using System.Reflection;

using Carter;

using FluentValidation;

using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Holefeeder.Application.Features.Cashflows.Queries;

public class GetUpcoming : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/cashflows/get-upcoming",
                async (Request request, IMediator mediator, HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var (total, viewModels) = await mediator.Send(request, cancellationToken);
                    ctx.Response.Headers.Add("X-Total-Count", $"{total}");
                    return Results.Ok(viewModels);
                })
            .Produces<IEnumerable<UpcomingViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Cashflows))
            .WithName(nameof(GetUpcoming))
            .RequireAuthorization();
    }

    public record Request(DateTime From, DateTime To) : IRequest<QueryResult<UpcomingViewModel>>
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            const string fromKey = "from";
            const string toKey = "to";

            var hasFrom = DateTime.TryParse(context.Request.Query[fromKey], out var from);
            var hasTo = DateTime.TryParse(context.Request.Query[toKey], out var to);

            var result = new Request(hasFrom ? from : DateTime.MinValue, hasTo ? to : DateTime.MaxValue);

            return ValueTask.FromResult<Request?>(result);
        }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.From).NotEmpty();
            RuleFor(command => command.To)
                .NotEmpty()
                .GreaterThanOrEqualTo(command => command.From)
                .WithMessage($"{nameof(Request.To)} must be greater or equal to {nameof(Request.To)}.");
        }
    }

    public class Handler : IRequestHandler<Request, QueryResult<UpcomingViewModel>>
    {
        private readonly IUserContext _userContext;
        private readonly IUpcomingQueriesRepository _repository;

        public Handler(IUserContext userContext, IUpcomingQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<QueryResult<UpcomingViewModel>> Handle(Request query, CancellationToken cancellationToken)
        {
            var results =
                (await _repository.GetUpcomingAsync(_userContext.UserId, query.From, query.To, cancellationToken))
                .ToList();

            return new QueryResult<UpcomingViewModel>(results.Count, results);
        }
    }
}
