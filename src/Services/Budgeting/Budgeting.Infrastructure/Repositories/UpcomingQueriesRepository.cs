using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Budgeting.Application.Contracts;
using DrifterApps.Holefeeder.Budgeting.Application.Models;
using DrifterApps.Holefeeder.Budgeting.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Repositories
{
    public class UpcomingQueriesRepository : RepositoryRoot, IUpcomingQueriesRepository
    {
        public UpcomingQueriesRepository(IMongoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UpcomingViewModel>> GetUpcomingAsync(Guid userId, DateTime startDate, DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            var transactionCollection = await DbContext.GetTransactionsAsync(cancellationToken);
            var cashflowCollection = await DbContext.GetCashflowsAsync(cancellationToken);
            var accountCollection = await DbContext.GetAccountsAsync(cancellationToken);
            var categoryCollection = await DbContext.GetCategoriesAsync(cancellationToken);

            var pastCashflows = await transactionCollection.AsQueryable()
                .Where(x => x.UserId == userId && !string.IsNullOrEmpty(x.Cashflow))
                .OrderByDescending(x => x.Date)
                .GroupBy(x => x.Cashflow)
                .Select(g => new
                {
                    Cashflow = g.Key, LastPaidDate = g.First().Date, LastCashflowDate = g.First().CashflowDate
                }).ToListAsync(cancellationToken);

            var cashflows = await cashflowCollection
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
            var accounts = await accountCollection
                .AsQueryable()
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
            var categories = await categoryCollection
                .AsQueryable()
                .Where(x => x.UserId == userId)
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
                        t?.LastPaidDate,
                        t?.LastCashflowDate
                    })
                .SelectMany(x =>
                {
                    var dates = new List<DateTime>();

                    var nextDate =
                        x.Cashflow.IntervalType.NextDate(x.Cashflow.EffectiveDate, startDate, x.Cashflow.Frequency);
                    if (IsUnpaid(x.Cashflow.EffectiveDate, nextDate, x.LastPaidDate, x.LastCashflowDate))
                    {
                        dates.Add(nextDate);
                    }

                    var date = nextDate;
                    while (IsUnpaid(x.Cashflow.EffectiveDate, date, x.LastPaidDate, x.LastCashflowDate) &&
                           date > x.Cashflow.EffectiveDate)
                    {
                        date = x.Cashflow.IntervalType.PreviousDate(x.Cashflow.EffectiveDate, date,
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
                            new AccountInfoViewModel(x.Account.Id, x.Account.Name, x.Account.MongoId), x.Cashflow.Tags));
                }).Where(x => x.Date <= endDate)
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
