using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Application.Contracts;
using DrifterApps.Holefeeder.Application.Models;
using DrifterApps.Holefeeder.Domain.Extensions;
using DrifterApps.Holefeeder.Infrastructure.MongoDB.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Infrastructure.MongoDB.Repositories
{
    public class UpcomingQueriesRepository : RepositoryRoot, IUpcomingQueriesRepository
    {
        public UpcomingQueriesRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime from, DateTime to,
            CancellationToken cancellationToken = default)
        {
            var transactionCollection = await DbContext.GetTransactionsAsync(cancellationToken);
            var cashflowCollection = await DbContext.GetCashflowsAsync(cancellationToken);
            var accountCollection = await DbContext.GetAccountsAsync(cancellationToken);
            var categoryCollection = await DbContext.GetCategoriesAsync(cancellationToken);

            var mongoUserId = await GetUserMongoId(userId, cancellationToken);

            if (string.IsNullOrWhiteSpace(mongoUserId))
                return new List<UpcomingViewModel>();

            var pastCashflows = await transactionCollection.AsQueryable()
                .Where(x => x.UserId == mongoUserId && !string.IsNullOrEmpty(x.Cashflow))
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Cashflow)
                .Select(g => new
                {
                    Cashflow = g.Key, LastPaidDate = g.First().Date, LastCashflowDate = g.First().CashflowDate
                }).ToListAsync(cancellationToken);

            var cashflows = await cashflowCollection
                .AsQueryable()
                .Where(x => x.UserId == mongoUserId)
                .ToListAsync(cancellationToken);
            var accounts = await accountCollection
                .AsQueryable()
                .Where(x => x.UserId == mongoUserId)
                .ToListAsync(cancellationToken);
            var categories = await categoryCollection
                .AsQueryable()
                .Where(x => x.UserId == mongoUserId)
                .ToListAsync(cancellationToken);

            var upcomingCashflows = cashflows
                .Where(x => !x.Inactive)
                .Join(accounts,
                    c => c.Account,
                    a => a.MongoId,
                    (c, a) => new {Cashflow = c, Account = a})
                .Join(categories,
                    j => j.Cashflow.Category,
                    c => c.MongoId,
                    (j, c) => new {j.Cashflow, j.Account, Category = c});

            var results =
                (from c in upcomingCashflows
                    join t in pastCashflows on c.Cashflow.MongoId equals t.Cashflow into u
                    from t in u.DefaultIfEmpty()
                    select new
                    {
                        c.Cashflow,
                        c.Account,
                        c.Category,
                        t.LastPaidDate,
                        t.LastCashflowDate
                    }).SelectMany(x =>
                {
                    var dates = new List<DateTime>();

                    var nextDate =
                        x.Cashflow.EffectiveDate.NextDate(from, x.Cashflow.IntervalType, x.Cashflow.Frequency);
                    if (IsUnpaid(x.Cashflow.EffectiveDate, nextDate, x.LastPaidDate, x.LastCashflowDate))
                    {
                        dates.Add(nextDate);
                    }

                    var date = nextDate;
                    while (IsUnpaid(x.Cashflow.EffectiveDate, date, x.LastPaidDate, x.LastCashflowDate) &&
                           date > x.Cashflow.EffectiveDate)
                    {
                        date = x.Cashflow.EffectiveDate.PreviousDate(date, x.Cashflow.IntervalType,
                            x.Cashflow.Frequency);
                        if (IsUnpaid(x.Cashflow.EffectiveDate, date, x.LastPaidDate, x.LastCashflowDate))
                        {
                            dates.Add(date);
                        }
                    }

                    return dates.Select(d =>
                        new UpcomingViewModel(x.Cashflow.Id, d, x.Cashflow.Amount, x.Cashflow.Description,
                            new CategoryInfoViewModel(x.Category.Id, x.Category.Name, x.Category.Type,
                                x.Category.Color),
                            new AccountInfoViewModel(x.Account.Id, x.Account.Name), x.Cashflow.Tags));
                }).Where(x => x.Date <= to)
                .OrderBy(x => x.Date);

            return results;
        }

        private static bool IsUnpaid(DateTime effectiveDate, DateTime nextDate, DateTime? lastPaidDate,
            DateTime? lastCashflowDate) =>
            !lastPaidDate.HasValue
                ? (nextDate >= effectiveDate)
                : (nextDate > lastPaidDate && nextDate > lastCashflowDate);
    }
}
