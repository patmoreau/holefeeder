using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using DrifterApps.Holefeeder.Framework.Mongo.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork;
using DrifterApps.Holefeeder.Framework.SeedWork.Application;
using DrifterApps.Holefeeder.Framework.SeedWork.Infrastructure;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class AccountQueriesRepository : RepositoryRoot, IAccountQueriesRepository
    {
        public AccountQueriesRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AccountViewModel>> GetAccountsAsync(Guid userId, QueryParams queryParams,
            CancellationToken cancellationToken)
        {
            queryParams.ThrowIfNull(nameof(queryParams));
            
            var accountCollection = await DbContext.GetAccountsAsync(cancellationToken);
            var transactionCollection = await DbContext.GetTransactionsAsync(cancellationToken);
            var categoryCollection = await DbContext.GetCategoriesAsync(cancellationToken);

            var accounts = await accountCollection
                .AsQueryable()
                .Where(t => t.UserId == userId)
                .Filter(queryParams.Filter)
                .ToListAsync(cancellationToken);

            var summaries = await accountCollection.AsQueryable()
                .Where(t => t.UserId == userId)
                .Filter(queryParams.Filter)
                .Join(transactionCollection.AsQueryable(),
                    a => a.MongoId,
                    t => t.Account,
                    (a, t) => new {a.Id, t.Category, t.Amount, t.Date})
                .GroupBy(t => new {t.Id, t.Category})
                .Select(g => new
                {
                    g.Key.Id,
                    g.Key.Category,
                    TransactionCount = g.Count(),
                    TransactionAmount = g.Sum(x => x.Amount),
                    LastTransactionDate = g.Max(x => x.Date)
                })
                .Join(categoryCollection.AsQueryable(),
                    s => s.Category,
                    c => c.MongoId,
                    (s, c) => new
                    {
                        s.Id,
                        CategoryType = c.Type,
                        s.TransactionCount,
                        s.TransactionAmount,
                        s.LastTransactionDate
                    })
                .ToListAsync(cancellationToken);

            var results = accounts.AsQueryable()
                .GroupJoin(summaries,
                    a => a.Id,
                    s => s.Id,
                    (a, s) => new {Account = a, Summary = s})
                .Select(
                    x => new AccountViewModel
                    (
                        x.Account.Id,
                        x.Account.Type,
                        x.Account.Name,
                        x.Summary.Sum(s => s.TransactionCount),
                        x.Account.OpenBalance + (
                            x.Summary.Sum(s => s.TransactionAmount * s.CategoryType.Multiplier) *
                            x.Account.Type.Multiplier),
                        x.Summary.Any() ? x.Summary.Max(s => s.LastTransactionDate) : x.Account.OpenDate,
                        x.Account.Description,
                        x.Account.Favorite
                    ))
                .Sort(queryParams.Sort)
                .Skip(queryParams.Offset)
                .Take(queryParams.Limit)
                .ToList();

            return results;
        }
    }
}
