using Dapper;

using Holefeeder.Application.Features.Transactions;
using Holefeeder.Application.Models;
using Holefeeder.Application.SeedWork;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class TransactionQueriesRepository : ITransactionQueriesRepository
{
    private readonly IHolefeederContext _context;

    public TransactionQueriesRepository(IHolefeederContext context)
    {
        _context = context;
    }

    public async Task<(int Total, IEnumerable<TransactionInfoViewModel> Items)> FindAsync(Guid userId,
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
                new {Offset = queryParams.Offset + 1, Limit = queryParams.Offset + queryParams.Limit});
        var countTemplate = builder.AddTemplate(queryCountTemplate);

        builder.Where($"user_id = @{nameof(userId)}", new {userId})
            .Filter(queryParams.Filter)
            .Sort(queryParams.Sort);

        var connection = _context.Connection;

        var transactions = await connection
            .QueryAsync<TransactionEntity, AccountEntity, CategoryEntity, TransactionEntity>(
                selectTemplate.RawSql,
                (transaction, account, category) => transaction with {Account = account, Category = category},
                selectTemplate.Parameters,
                splitOn: "id,id")
            .ConfigureAwait(false);
        var count = await connection.ExecuteScalarAsync<int>(countTemplate.RawSql, countTemplate.Parameters);

        return new ValueTuple<int, IEnumerable<TransactionInfoViewModel>>(count,
            TransactionMapper.MapToDto(transactions));
    }

    public async Task<TransactionInfoViewModel?> FindByIdAsync(Guid userId, Guid id,
        CancellationToken cancellationToken)
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

        builder.Where($"T.id = @{nameof(id)} AND T.user_id = @{nameof(userId)}", new {id, userId});

        var transactions = await connection
            .QueryAsync<TransactionEntity, AccountEntity, CategoryEntity, TransactionEntity>(
                selectTemplate.RawSql,
                (transaction, account, category) => transaction with {Account = account, Category = category},
                selectTemplate.Parameters,
                splitOn: "id,id")
            .ConfigureAwait(false);

        return TransactionMapper.MapToDtoOrNull(transactions.FirstOrDefault());
    }
}
