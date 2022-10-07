using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Transactions.Queries;

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
            .WithTags(nameof(Transactions))
            .WithName(nameof(GetCashflow))
            .RequireAuthorization();
    }

    internal record Request(Guid Id) : IRequest<CashflowInfoViewModel>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, CashflowInfoViewModel>
    {
        private readonly ICashflowQueriesRepository _repository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, ICashflowQueriesRepository repository)
        {
            _userContext = userContext;
            _repository = repository;
        }

        public async Task<CashflowInfoViewModel> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = await _repository.FindByIdAsync(_userContext.UserId, request.Id, cancellationToken);
            if (result is null)
            {
                throw new CashflowNotFoundException(request.Id);
            }

            return result;
        }
    }
}
