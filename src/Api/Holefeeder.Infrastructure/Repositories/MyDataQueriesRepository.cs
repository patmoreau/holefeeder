using Holefeeder.Application.Features.MyData;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class MyDataQueriesRepository : IMyDataQueriesRepository
{
    private readonly IHolefeederContext _context;

    public MyDataQueriesRepository(IHolefeederContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MyDataAccountDto>> ExportAccountsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<AccountEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(AccountMapper.MapToMyDataAccountDto);
    }

    public async Task<IEnumerable<MyDataCategoryDto>> ExportCategoriesAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<CategoryEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(CategoryMapper.MapToMyDataCategoryDto);
    }

    public async Task<IEnumerable<MyDataCashflowDto>> ExportCashflowsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<CashflowEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(CashflowMapper.MapToMyDataCashflowDto);
    }

    public async Task<IEnumerable<MyDataTransactionDto>> ExportTransactionsAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<TransactionEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(TransactionMapper.MapToMyDataTransactionDto);
    }
}
