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
        app.MapGet("api/v2/my-data/export-data", async (IMediator mediator) =>
            {
                var requestResult = await mediator.Send(new Request());
                return Results.Ok(requestResult);
            })
            .Produces<ExportDataDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ExportData))
            .RequireAuthorization(Policies.ReadUser);

    internal record Request : IRequest<ExportDataDto>;

    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, ExportDataDto>
    {
        private readonly BudgetingContext _context = context;
        private readonly IUserContext _userContext = userContext;

        public async Task<ExportDataDto> Handle(Request request, CancellationToken cancellationToken)
        {
            var accounts = (await _context.Accounts
                    .Where(e => e.UserId == _userContext.Id)
                    .ToListAsync(cancellationToken))
                .Select(AccountMapper.MapToMyDataAccountDto);
            var categories = (await _context.Categories
                    .Where(e => e.UserId == _userContext.Id)
                    .ToListAsync(cancellationToken))
                .Select(CategoryMapper.MapToMyDataCategoryDto);
            var cashflows = (await _context.Cashflows
                    .Where(e => e.UserId == _userContext.Id)
                    .Include(e => e.Account)
                    .Include(e => e.Category)
                    .ToListAsync(cancellationToken))
                .Select(CashflowMapper.MapToMyDataCashflowDto);
            var transactions = (await _context.Transactions
                    .Where(e => e.UserId == _userContext.Id)
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
}
