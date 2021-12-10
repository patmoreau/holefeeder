using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Dapper;

using DrifterApps.Holefeeder.Budgeting.Domain.BoundedContext.AccountContext;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Entities;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Mapping;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using Framework.Dapper.SeedWork.Extensions;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IHolefeederContext _context;
    private readonly AccountMapper _accountMapper;

    public IUnitOfWork UnitOfWork => _context;

    public AccountRepository(IHolefeederContext context, AccountMapper accountMapper)
    {
        _context = context;
        _accountMapper = accountMapper;
    }

    public async Task<Account?> FindByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var cashflows = new List<Guid>();

        var account = (await connection
                .QueryAsync<AccountEntity, Guid?, AccountEntity>(@"
SELECT 
    a.*, c.id
FROM accounts a
LEFT OUTER JOIN cashflows c on c.account_id = a.id
WHERE a.id = @Id AND a.user_id = @UserId;
",
                    (account, cashflowId) =>
                    {
                        if (cashflowId is not null)
                        {
                            cashflows.Add(cashflowId.Value);
                        }

                        return account;
                    },
                    new { Id = id, UserId = userId },
                    splitOn: "id")
                .ConfigureAwait(false))
            .Distinct()
            .SingleOrDefault();

        return _accountMapper.MapToModelOrNull(account, cashflows);
    }

    public async Task<Account?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<AccountEntity>(new {Name = name, UserId = userId})
            .ConfigureAwait(false);

        return _accountMapper.MapToModelOrNull(schema.FirstOrDefault());
    }

    public async Task SaveAsync(Account account, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = account.Id;
        var userId = account.UserId;

        var entity = await transaction.FindByIdAsync<AccountEntity>(new { Id = id, UserId = userId });

        if (entity is null)
        {
            await transaction.InsertAsync(_accountMapper.MapToEntity(account))
                .ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(_accountMapper.MapToEntity(account)).ConfigureAwait(false);
        }
    }
}
