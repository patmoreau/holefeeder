using DrifterApps.Seeds.Application;
using DrifterApps.Seeds.Application.Mediatr;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts.Exceptions;
using Holefeeder.Domain.Features.Accounts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Commands;

public class ModifyAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/accounts/modify-account",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    _ = await mediator.Send(request, cancellationToken);
                    return Results.NoContent();
                })
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(ModifyAccount))
            .RequireAuthorization();

    internal class Handler : IRequestHandler<Request, Unit>
    {
        private readonly BudgetingContext _context;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            Account? exists = await _context.Accounts
                .SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == _userContext.Id, cancellationToken);
            if (exists is null)
            {
                throw new AccountNotFoundException(request.Id);
            }

            _context.Update(exists with
            {
                Name = request.Name,
                Description = request.Description,
                OpenBalance = request.OpenBalance
            });

            return Unit.Value;
        }
    }

    internal record Request(Guid Id, string Name, decimal OpenBalance, string Description) : IRequest<Unit>, IUnitOfWorkRequest;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEmpty();
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
        }
    }
}
