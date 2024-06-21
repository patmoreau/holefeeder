using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Transactions.Exceptions;
using Holefeeder.Application.Models;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Transactions.Queries;

public class GetCashflow : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
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

    internal record Request(Guid Id) : IRequest<CashflowInfoViewModel>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Id).NotEmpty();
    }

    internal class Handler(IUserContext userContext, BudgetingContext context)
        : IRequestHandler<Request, CashflowInfoViewModel>
    {
        public async Task<CashflowInfoViewModel> Handle(Request request, CancellationToken cancellationToken)
        {
            var result = await context.Cashflows
                .Include(x => x.Account).Include(x => x.Category)
                .SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
            if (result is null)
            {
                throw new CashflowNotFoundException(request.Id);
            }

            return CashflowMapper.MapToDto(result);
        }
    }
}
