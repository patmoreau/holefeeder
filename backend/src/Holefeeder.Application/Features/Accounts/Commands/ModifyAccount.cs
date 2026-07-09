using System.ComponentModel.DataAnnotations;

using DrifterApps.Seeds.Application.EndpointFilters;
using DrifterApps.Seeds.FluentResult;

using Holefeeder.Application.Context;
using Holefeeder.Application.Extensions;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.ValueObjects;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Accounts.Commands;

public class ModifyAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/accounts/modify-account",
                async (Request request, IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
                {
                    var result =  await Handle(request, userContext, context, cancellationToken);
                    return result switch
                        {
                            { IsFailure: true } => result.Error.ToProblem(),
                            _ => Results.NoContent()
                        };
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .AddEndpointFilter<UnitOfWorkFilter>()
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(nameof(Accounts))
            .WithName(nameof(ModifyAccount))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<Nothing>> Handle(Request request, IUserContext userContext,
        BudgetingContext context, CancellationToken cancellationToken)
    {
        var exists = await context.Accounts
            .SingleOrDefaultAsync(x => x.Id == request.Id && x.UserId == userContext.Id, cancellationToken);
        if (exists is null)
        {
            return AccountErrors.NotFound(request.Id);
        }

        var result = exists.Modify(name: request.Name, openBalance: request.OpenBalance,
            description: request.Description);
        if (result.IsFailure)
        {
            return result.Error;
        }

        context.Update(result.Value);

        return Nothing.Value;
    }

    internal record Request([Required]AccountId Id, [MinLength(1), MaxLength(255)]string Name, Money OpenBalance, string Description);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(command => command.Id).NotNull().NotEqual(AccountId.Empty);
            RuleFor(command => command.Name).NotNull().NotEmpty().Length(1, 255);
        }
    }
}
