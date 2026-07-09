using System.Collections.Immutable;

using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts;
using Holefeeder.Application.Features.Categories;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.Transactions;
using Holefeeder.Application.UserContext;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.MyData.Queries;

public sealed class ExportData : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/my-data/export-data", async (IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken) =>
            {
                var requestResult = await Handle(userContext, context, cancellationToken);
                return Results.Ok(requestResult);
            })
            .Produces<ExportDataDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ExportData))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<ExportDataDto> Handle(IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var accounts = (await context.Accounts
                .Where(e => e.UserId == userContext.Id)
                .ToListAsync(cancellationToken))
            .Select(AccountMapper.MapToMyDataAccountDto);
        var categories = (await context.Categories
                .Where(e => e.UserId == userContext.Id)
                .ToListAsync(cancellationToken))
            .Select(CategoryMapper.MapToMyDataCategoryDto);
        var cashflows = (await context.Cashflows
                .Where(e => e.UserId == userContext.Id)
                .Include(e => e.Account)
                .Include(e => e.Category)
                .ToListAsync(cancellationToken))
            .Select(CashflowMapper.MapToMyDataCashflowDto);
        var transactions = (await context.Transactions
                .Where(e => e.UserId == userContext.Id)
                .Include(e => e.Account)
                .Include(e => e.Category)
                .ToListAsync(cancellationToken))
            .Select(TransactionMapper.MapToMyDataTransactionDto);

        return new ExportDataDto(accounts.ToImmutableArray(),
            categories.ToImmutableArray(),
            cashflows.ToImmutableArray(),
            transactions.ToImmutableArray());
    }
}
