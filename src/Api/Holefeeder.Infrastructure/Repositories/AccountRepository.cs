using Dapper;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.SeedWork;
using Holefeeder.Infrastructure.Context;
using Holefeeder.Infrastructure.Entities;
using Holefeeder.Infrastructure.Extensions;
using Holefeeder.Infrastructure.Mapping;

namespace Holefeeder.Infrastructure.Repositories;

internal class AccountRepository : IAccountRepository
{
    private readonly IHolefeederContext _context;

    public AccountRepository(IHolefeederContext context)
    {
        _context = context;
    }

    public IUnitOfWork UnitOfWork => _context;

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
                    new {Id = id, UserId = userId},
                    splitOn: "id")
                .ConfigureAwait(false))
            .Distinct()
            .SingleOrDefault();

        return AccountMapper.MapToModelOrNull(account, cashflows);
    }

    public async Task<Account?> FindByNameAsync(string name, Guid userId, CancellationToken cancellationToken)
    {
        var connection = _context.Connection;

        var schema = await connection
            .FindAsync<AccountEntity>(new {Name = name, UserId = userId})
            .ConfigureAwait(false);

        return AccountMapper.MapToModelOrNull(schema.FirstOrDefault());
    }

    public async Task SaveAsync(Account account, CancellationToken cancellationToken)
    {
        var transaction = _context.Transaction;

        var id = account.Id;
        var userId = account.UserId;

        var entity = await transaction.FindByIdAsync<AccountEntity>(new {Id = id, UserId = userId});

        if (entity is null)
        {
            await transaction.InsertAsync(AccountMapper.MapToEntity(account))
                .ConfigureAwait(false);
        }
        else
        {
            await transaction.UpdateAsync(AccountMapper.MapToEntity(account)).ConfigureAwait(false);
        }
    }
}
