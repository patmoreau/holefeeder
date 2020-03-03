using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.ResourcesAccess;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Extensions;
using System.Threading;

namespace DrifterApps.Holefeeder.Business
{
    public class CashflowsService : BaseOwnedService<CashflowEntity>, ICashflowsService
    {
        private readonly ICashflowsRepository _repository;
        private readonly ITransactionsService _transactionsService;
        private readonly IAccountsService _accountsService;
        private readonly ICategoriesService _categoriesService;

        public CashflowsService(ICashflowsRepository repository, ITransactionsService transactionsService, IAccountsService accountsService, ICategoriesService categoriesService) : base(repository)
        {
            _repository = repository.ThrowIfNull(nameof(repository));
            _transactionsService = transactionsService.ThrowIfNull(nameof(transactionsService));
            _accountsService = accountsService.ThrowIfNull(nameof(accountsService));
            _categoriesService = categoriesService.ThrowIfNull(nameof(categoriesService));
        }

        public async Task<IEnumerable<UpcomingEntity>> GetUpcomingAsync(string userId, (DateTime From, DateTime To) interval, CancellationToken cancellationToken = default)
        {
            var pastCashflows = (await _transactionsService.FindAsync(userId, new QueryParams(null, null, sort: new[] { "-date" }, null), cancellationToken).ConfigureAwait(false))
                .GroupBy(t => t.Cashflow)
                .Select(g => new
                {
                    Cashflow = g.Key,
                    LastPaidDate = g.First().Date,
                    LastCashflowDate = g.First().CashflowDate
                });

            var cashflows = await _repository.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false);
            var accounts = await _accountsService.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false);
            var categories = await _categoriesService.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false);

            var upcomingCashflows = from c in cashflows
                                    join a in accounts on c.Account equals a.Id
                                    join cat in categories on c.Category equals cat.Id
                                    join t in pastCashflows on c.Id equals t.Cashflow into u
                                    from t in u.DefaultIfEmpty()
                                    select new { Cashflow = c, Account = a, Category = cat, t?.LastPaidDate, t?.LastCashflowDate };

            var results = upcomingCashflows.SelectMany(x =>
            {
                var dates = new List<DateTime>();

                var nextDate = x.Cashflow.EffectiveDate.NextDate(interval.From, x.Cashflow.IntervalType, x.Cashflow.Frequency);
                if (IsUnpaid(x.Cashflow.EffectiveDate, nextDate, x.LastPaidDate, x.LastCashflowDate))
                {
                    dates.Add(nextDate);
                }

                var date = nextDate;
                while (IsUnpaid(x.Cashflow.EffectiveDate, date, x.LastPaidDate, x.LastCashflowDate) && date > x.Cashflow.EffectiveDate)
                {
                    date = x.Cashflow.EffectiveDate.PreviousDate(date, x.Cashflow.IntervalType, x.Cashflow.Frequency);
                    if (IsUnpaid(x.Cashflow.EffectiveDate, date, x.LastPaidDate, x.LastCashflowDate))
                    {
                        dates.Add(date);
                    }
                }

                return dates.Select(d =>
                    new UpcomingEntity(x.Cashflow.Id, d, x.LastPaidDate, x.LastCashflowDate, x.Cashflow.IntervalType, x.Cashflow.Frequency, x.Cashflow.Amount, x.Cashflow.Description, x.Cashflow.Tags,
                        new CategoryInfoEntity(x.Category.Id, x.Category.Name, x.Category.Type, x.Category.Color),
                        new AccountInfoEntity(x.Account.Id, x.Account.Name)));
            }).Where(x => x.Date <= interval.To)
              .OrderBy(x => x.Date);

            return results;
        }

        private static bool IsUnpaid(DateTime effectiveDate, DateTime nextDate, DateTime? lastPaidDate, DateTime? lastCashflowDate) =>
            !lastPaidDate.HasValue ? (nextDate >= effectiveDate) : (nextDate > lastPaidDate && nextDate > lastCashflowDate);
    }
}