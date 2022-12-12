using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Application.SeedWork;
using Holefeeder.Domain.Features.Accounts;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Commands;

public class OpenAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/v2/accounts/open-account",
                async (Request request, IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Send(request, cancellationToken);
                    return Results.CreatedAtRoute(nameof(GetAccount), new {Id = result}, new {Id = result});
                })
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Accounts))
            .WithName(nameof(OpenAccount))
            .RequireAuthorization();
    }

    internal record Request(AccountType Type, string Name, DateTime OpenDate, decimal OpenBalance, string Description)
        : ICommandRequest<Guid>;

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Type).NotNull();
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
            RuleFor(command => command.OpenDate).NotEmpty();
        }
    }

    internal class Handler : IRequestHandler<Request, Guid>
    {
        private readonly IUserContext _userContext;
        private readonly BudgetingContext _context;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<Guid> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await _context.Accounts.AnyAsync(x => x.Name == request.Name && x.UserId == _userContext.UserId,
                    cancellationToken))
            {
                throw new AccountDomainException($"Name '{request.Name}' already exists.");
            }

            var account = Account.Create(request.Type, request.Name, request.OpenBalance, request.OpenDate,
                request.Description, _userContext.UserId);

            await _context.Accounts.AddAsync(account, cancellationToken);

            return account.Id;
        }
    }
}
