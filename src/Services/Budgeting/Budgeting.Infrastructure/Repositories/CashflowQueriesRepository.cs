using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Application.Cashflows;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class CashflowQueriesRepository : ICashflowQueriesRepository
{
    private readonly CashflowMapper _cashflowMapper;
    private readonly IHolefeederContext _context;

    public CashflowQueriesRepository(IHolefeederContext context, CashflowMapper cashflowMapper)
    {
        _context = context;
        _cashflowMapper = cashflowMapper;
    }

    public async Task<(int Total, IEnumerable<CashflowInfoViewModel> Items)> FindAsync(Guid userId,
        QueryParams queryParams, CancellationToken cancellationToken)
    {
        const string queryTemplate = @"
SELECT X.*, A.*, CA.* FROM (
    SELECT C.*, ROW_NUMBER() OVER (/**orderby**/) AS row_nb 
    FROM cashflows C
    /**where**/
) AS X 
INNER JOIN accounts A ON A.id = X.account_id
INNER JOIN categories CA on CA.id = X.category_id
WHERE row_nb BETWEEN @offset AND @limit
ORDER BY row_nb;
";
        const string queryCountTemplate = @"SELECT COUNT(*) FROM cashflows /**where**/";

        var builder = new SqlBuilder();
        var selectTemplate =
            builder.AddTemplate(queryTemplate,
                new {Offset = queryParams.Offset + 1, Limit = queryParams.Offset + queryParams.Limit});
        var countTemplate = builder.AddTemplate(queryCountTemplate);

        builder.Where($"user_id = @{nameof(userId)}", new {userId})
            .Filter(queryParams.Filter)
            .Sort(queryParams.Sort);

        var connection = _context.Connection;

        var cashflows = await connection
            .QueryAsync<CashflowEntity, AccountEntity, CategoryEntity, CashflowEntity>(
                selectTemplate.RawSql,
                (cashflow, account, category) => cashflow with {Account = account, Category = category},
                selectTemplate.Parameters,
                splitOn: "id,id")
            .ConfigureAwait(false);
        var count = await connection.ExecuteScalarAsync<int>(countTemplate.RawSql, countTemplate.Parameters);

        return new ValueTuple<int, IEnumerable<CashflowInfoViewModel>>(count,
            _cashflowMapper.MapToDto(cashflows));
    }

    public async Task<CashflowInfoViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        const string queryTemplate = @"
SELECT C.*, A.*, CA.* 
FROM cashflows C
INNER JOIN accounts A ON A.id = C.account_id
INNER JOIN categories CA on CA.id = C.category_id
/**where**/
";
        var connection = _context.Connection;

        var builder = new SqlBuilder();
        var selectTemplate =
            builder.AddTemplate(queryTemplate);

        builder.Where($"C.id = @{nameof(id)} AND C.user_id = @{nameof(userId)}", new {id, userId});

        var cashflows = await connection
            .QueryAsync<CashflowEntity, AccountEntity, CategoryEntity, CashflowEntity>(
                selectTemplate.RawSql,
                (cashflow, account, category) => cashflow with {Account = account, Category = category},
                selectTemplate.Parameters,
                splitOn: "id,id")
            .ConfigureAwait(false);
        return _cashflowMapper.MapToDtoOrNull(cashflows.FirstOrDefault());
    }
}
