using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Application.Accounts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class AccountQueriesRepository : IAccountQueriesRepository
{
    private readonly IHolefeederContext _context;
    private readonly IMapper _mapper;

    public AccountQueriesRepository(IHolefeederContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Task<(int Total, IEnumerable<AccountViewModel> Items)> FindAsync(Guid userId, QueryParams queryParams,
        CancellationToken cancellationToken)
    {
        if (queryParams is null)
        {
            throw new ArgumentNullException(nameof(queryParams));
        }

        return FindInternalAsync(userId, queryParams);
    }

    private async Task<(int Total, IEnumerable<AccountViewModel> Items)> FindInternalAsync(Guid userId, QueryParams queryParams)
    {
        const string queryTemplate = @"
SELECT X.* FROM (
    SELECT A.*, ROW_NUMBER() OVER (/**orderby**/) AS row_nb 
    FROM accounts A
    /**where**/
    GROUP BY A.Id
) AS X 
WHERE row_nb BETWEEN @Offset AND @Limit;
";
        const string queryCountTemplate = @"SELECT COUNT(*) FROM accounts /**where**/";

        var builder = new SqlBuilder();
        var selectTemplate =
            builder.AddTemplate(queryTemplate,
                new { Offset = queryParams.Offset + 1, Limit = queryParams.Offset + queryParams.Limit });
        var countTemplate = builder.AddTemplate(queryCountTemplate);

        builder.Where($"user_id = @{nameof(userId)}", new { userId })
            .Filter(queryParams.Filter)
            .Sort(queryParams.Sort);

        var connection = _context.Connection;

        var accounts = (await connection.QueryAsync<AccountEntity>(selectTemplate.RawSql,
                    selectTemplate.Parameters)
                .ConfigureAwait(false))
            .ToList();

        var count = await connection.ExecuteScalarAsync<int>(countTemplate.RawSql, countTemplate.Parameters);

        var transactions = await GetTransactions(connection, accounts);

        var accountList = accounts.Select(account =>
                BuildAccountViewModel(account, transactions.Where(t => t.AccountId == account.Id).ToList()))
            .ToList();

        return new ValueTuple<int, IEnumerable<AccountViewModel>>(count, accountList);
    }

    public async Task<AccountViewModel?> FindByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var account = await connection.QuerySingleAsync<AccountEntity>(
                @"SELECT * FROM accounts WHERE id = @Id AND user_id = @UserId;", new { Id = id, UserId = userId })
            .ConfigureAwait(false);

        var transactions = await GetTransactions(connection, new[] { account });

        return BuildAccountViewModel(account, transactions);
    }

    public async Task<bool> IsAccountActive(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        const string selectIsActive = @"
SELECT COUNT(*)
FROM accounts
WHERE id = @Id AND user_id = @UserId AND inactive = 0;
";

        var isActive = await _context.Connection
            .ExecuteScalarAsync<int>(selectIsActive, new { Id = id, UserId = userId })
            .ConfigureAwait(false);
        return isActive > 0;
    }

    private static AccountViewModel BuildAccountViewModel(AccountEntity account,
        ICollection<TransactionEntity> transactions) =>
        new()
        {
            Id = account.Id,
            OpenBalance = account.OpenBalance,
            OpenDate = account.OpenDate,
            Balance = account.OpenBalance +
                      transactions.Sum(t => t.Amount * t.Category.Type.Multiplier * account.Type.Multiplier),
            Description = account.Description,
            Favorite = account.Favorite,
            Name = account.Name,
            TransactionCount = transactions.Count,
            Updated = transactions.Any() ? transactions.Max(t => t.Date) : account.OpenDate,
            Type = account.Type
        };

    private static async Task<IList<TransactionEntity>> GetTransactions(IDbConnection connection,
        IEnumerable<AccountEntity> accounts)
    {
        const string sql = @"
SELECT t.*, ca.*
FROM transactions t
LEFT OUTER JOIN categories ca ON ca.id = t.category_id
WHERE t.account_id IN @Ids";

        var transactions = await connection
            .QueryAsync<TransactionEntity, CategoryEntity, TransactionEntity>(sql,
                (t, c) => t with { Category = c },
                new { Ids = accounts.Select(a => a.Id) },
                splitOn: "id")
            .ConfigureAwait(false);

        return transactions.ToList();
    }
}
