using Carter;

using FluentValidation;

using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Transactions;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Holefeeder.Application.Features.Transactions.Commands;

public class CancelCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/cashflows/cancel",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Transactions))
            .WithName(nameof(CancelCashflow))
            .RequireAuthorization();
    }

    internal record Request : IRequest<Unit>
    {
        public Guid Id { get; init; }
    }

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Unit>
    {
        private readonly ICashflowRepository _cashflowRepository;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, ICashflowRepository cashflowRepository)
        {
            _userContext = userContext;
            _cashflowRepository = cashflowRepository;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            try
            {
                var exists =
                    await _cashflowRepository.FindByIdAsync(request.Id, _userContext.UserId, cancellationToken);
                if (exists is null)
                {
                    throw new CashflowNotFoundException(request.Id);
                }

                var cashflow = exists.Cancel();

                await _cashflowRepository.SaveAsync(cashflow, cancellationToken);

                await _cashflowRepository.UnitOfWork.CommitAsync(cancellationToken);

                return Unit.Value;
            }
            catch (TransactionDomainException)
            {
                _cashflowRepository.UnitOfWork.Dispose();
                throw;
            }
        }
    }
}
