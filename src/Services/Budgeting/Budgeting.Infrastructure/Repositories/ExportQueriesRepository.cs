using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DrifterApps.Holefeeder.Budgeting.Application.MyData;
using DrifterApps.Holefeeder.Budgeting.Application.MyData.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class ExportQueriesRepository : IExportQueriesRepository
{
    private readonly IHolefeederContext _context;
    private readonly AccountMapper _accountMapper;
    private readonly CategoryMapper _categoryMapper;
    private readonly CashflowMapper _cashflowMapper;
    private readonly TransactionMapper _transactionMapper;

    public ExportQueriesRepository(IHolefeederContext context, AccountMapper accountMapper,
        CategoryMapper categoryMapper, CashflowMapper cashflowMapper, TransactionMapper transactionMapper)
    {
        _context = context;
        _accountMapper = accountMapper;
        _categoryMapper = categoryMapper;
        _cashflowMapper = cashflowMapper;
        _transactionMapper = transactionMapper;
    }
    public async Task<IEnumerable<MyDataAccountDto>> ExportAccountsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<AccountEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(_accountMapper.MapToExportDto);
    }

    public async Task<IEnumerable<MyDataCategoryDto>> ExportCategoriesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<CategoryEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(_categoryMapper.MapToExportDto);
    }

    public async Task<IEnumerable<MyDataCashflowDto>> ExportCashflowsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<CashflowEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(_cashflowMapper.MapToExportDto);
    }

    public async Task<IEnumerable<MyDataTransactionDto>> ExportTransactionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<TransactionEntity>(new {UserId = userId})
            .ConfigureAwait(false);

        return schema.Select(_transactionMapper.MapToExportDto);
    }
}
