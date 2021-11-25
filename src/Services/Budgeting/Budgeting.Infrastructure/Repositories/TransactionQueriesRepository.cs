using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Application.Transactions;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class TransactionQueriesRepository : ITransactionQueriesRepository
{
    private readonly IHolefeederContext _context;
    private readonly IMapper _mapper;

    public TransactionQueriesRepository(IHolefeederContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(int Total, IEnumerable<TransactionViewModel> Items)> FindAsync(Guid userId,
        QueryParams queryParams, CancellationToken cancellationToken)
    {
        const string queryTemplate = @"
SELECT X.*, A.*, CA.* FROM (
    SELECT T.*, ROW_NUMBER() OVER (/**orderby**/) AS row_nb 
    FROM transactions T
    /**where**/
) AS X 
INNER JOIN accounts A ON A.id = X.account_id
INNER JOIN categories CA on CA.id = X.category_id
WHERE row_nb BETWEEN @offset AND @limit
ORDER BY row_nb;
";
        const string queryCountTemplate = @"SELECT COUNT(*) FROM transactions /**where**/";

        var builder = new SqlBuilder();
        var selectTemplate =
            builder.AddTemplate(queryTemplate,
                new { Offset = queryParams.Offset + 1, Limit = queryParams.Offset + queryParams.Limit });
        var countTemplate = builder.AddTemplate(queryCountTemplate);

        builder.Where($"user_id = @{nameof(userId)}", new { userId })
            .Filter(queryParams.Filter)
            .Sort(queryParams.Sort);

        var connection = _context.Connection;

        var transactions = await connection
            .QueryAsync<TransactionEntity, AccountEntity, CategoryEntity, TransactionEntity>(
                selectTemplate.RawSql,
                (transaction, account, category) => transaction with { Account = account, Category = category },
                selectTemplate.Parameters,
                splitOn: "id,id")
            .ConfigureAwait(false);
        var count = await connection.ExecuteScalarAsync<int>(countTemplate.RawSql, countTemplate.Parameters);

        return new ValueTuple<int, IEnumerable<TransactionViewModel>>(count,
            _mapper.Map<IEnumerable<TransactionViewModel>>(transactions));
    }

    public async Task<TransactionViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        const string queryTemplate = @"
SELECT T.*, A.*, CA.* 
FROM transactions T
INNER JOIN accounts A ON A.id = T.account_id
INNER JOIN categories CA on CA.id = T.category_id
/**where**/
";
        var connection = _context.Connection;

        var builder = new SqlBuilder();
        var selectTemplate =
            builder.AddTemplate(queryTemplate);

        builder.Where($"T.id = @{nameof(id)} AND T.user_id = @{nameof(userId)}", new { id, userId });

        var transactions = await connection
            .QueryAsync<TransactionEntity, AccountEntity, CategoryEntity, TransactionEntity>(
                selectTemplate.RawSql,
                (transaction, account, category) => transaction with { Account = account, Category = category },
                selectTemplate.Parameters,
                splitOn: "id,id")
            .ConfigureAwait(false);
        return _mapper.Map<TransactionViewModel>(transactions.FirstOrDefault());
    }
}
