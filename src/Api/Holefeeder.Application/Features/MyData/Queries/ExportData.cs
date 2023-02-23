using System.Collections.Immutable;
using Holefeeder.Application.Context;
using Holefeeder.Application.Features.Accounts;
using Holefeeder.Application.Features.Categories;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Application.Features.Transactions;
using Holefeeder.Application.SeedWork;
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
                ExportDataDto requestResult = await mediator.Send(new Request());
                return Results.Ok(requestResult);
            })
            .Produces<ExportDataDto>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(MyData))
            .WithName(nameof(ExportData))
            .RequireAuthorization();

    internal record Request : IRequest<ExportDataDto>;

    internal class Handler : IRequestHandler<Request, ExportDataDto>
    {
        private readonly BudgetingContext _context;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<ExportDataDto> Handle(Request request, CancellationToken cancellationToken)
        {
            IEnumerable<MyDataAccountDto> accounts = (await _context.Accounts
                    .Where(e => e.UserId == _userContext.UserId)
                    .ToListAsync(cancellationToken))
                .Select(AccountMapper.MapToMyDataAccountDto);
            IEnumerable<MyDataCategoryDto> categories = (await _context.Categories
                    .Where(e => e.UserId == _userContext.UserId)
                    .ToListAsync(cancellationToken))
                .Select(CategoryMapper.MapToMyDataCategoryDto);
            IEnumerable<MyDataCashflowDto> cashflows = (await _context.Cashflows
                    .Where(e => e.UserId == _userContext.UserId)
                    .Include(e => e.Account)
                    .Include(e => e.Category)
                    .ToListAsync(cancellationToken))
                .Select(CashflowMapper.MapToMyDataCashflowDto);
            IEnumerable<MyDataTransactionDto> transactions = (await _context.Transactions
                    .Where(e => e.UserId == _userContext.UserId)
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
