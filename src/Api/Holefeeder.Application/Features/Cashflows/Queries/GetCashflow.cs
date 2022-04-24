using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Cashflows.Exceptions;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Cashflows.Queries;

public class GetCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/cashflows/{id:guid}",
                async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var requestResult = await mediator.Send(new Request(id), cancellationToken);
                    return Results.Ok(requestResult);
                })
            .Produces<CashflowInfoViewModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Cashflows))
            .WithName(nameof(GetCashflow))
            .RequireAuthorization();
    }

    public record Request(Guid Id) : IRequest<CashflowInfoViewModel>;

    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Request, CashflowInfoViewModel>
    {
        private readonly IUserContext _userContext;
        private readonly ICashflowQueriesRepository _repository;

        public Handler(IUserContext userContext, ICashflowQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<CashflowInfoViewModel> Handle(Request query, CancellationToken cancellationToken)
        {
            var result = await _repository.FindByIdAsync(_userContext.UserId, query.Id, cancellationToken);
            if (result is null)
            {
                throw new CashflowNotFoundException(query.Id);
            }

            return result;
        }
    }
}
