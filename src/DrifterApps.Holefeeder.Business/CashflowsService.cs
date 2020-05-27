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
            var pastCashflows = (await _transactionsService.FindAsync(userId, new QueryParams(null, null, new[] { "-date" }, null), cancellationToken).ConfigureAwait(false))
                .GroupBy(t => t.Cashflow)
                .Select(g => (
                    Cashflow: g.Key,
                    LastPaidDate: g.First().Date,
                    LastCashflowDate: g.First().CashflowDate
                ));

            var cashflows = await _repository.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false);
            var accounts = await _accountsService.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false);
            var categories = await _categoriesService.FindAsync(userId, QueryParams.Empty, cancellationToken).ConfigureAwait(false);

            var upcomingCashflows = from c in cashflows.Where(c => !c.Inactive)
                                    join a in accounts on c.Account equals a.Id
                                    join cat in categories on c.Category equals cat.Id
                                    join t in pastCashflows on c.Id equals t.Cashflow into u
                                    from t in u.DefaultIfEmpty()
                                    select (Cashflow: c, Account: a, Category: cat, t.LastPaidDate, t.LastCashflowDate);

            var (dateTime, to) = interval;
            var results = upcomingCashflows.SelectMany(x =>
            {
                var dates = new List<DateTime>();

                var (cashflow, account, category, lastPaidDate, lastCashflowDate) = x;
                var nextDate = cashflow.EffectiveDate.NextDate(dateTime, cashflow.IntervalType, cashflow.Frequency);
                if (IsUnpaid(cashflow.EffectiveDate, nextDate, lastPaidDate, lastCashflowDate))
                {
                    dates.Add(nextDate);
                }

                var date = nextDate;
                while (IsUnpaid(cashflow.EffectiveDate, date, lastPaidDate, lastCashflowDate) && date > cashflow.EffectiveDate)
                {
                    date = cashflow.EffectiveDate.PreviousDate(date, cashflow.IntervalType, cashflow.Frequency);
                    if (IsUnpaid(cashflow.EffectiveDate, date, lastPaidDate, lastCashflowDate))
                    {
                        dates.Add(date);
                    }
                }

                return dates.Select(d =>
                    new UpcomingEntity(cashflow.Id, d, lastPaidDate, lastCashflowDate, cashflow.IntervalType, cashflow.Frequency, cashflow.Amount, cashflow.Description, cashflow.Tags,
                        new CategoryInfoEntity(category.Id, category.Name, category.Type, category.Color),
                        new AccountInfoEntity(account.Id, account.Name)));
            }).Where(x => x.Date <= to)
              .OrderBy(x => x.Date);

            return results;
        }

        private static bool IsUnpaid(DateTime effectiveDate, DateTime nextDate, DateTime? lastPaidDate, DateTime? lastCashflowDate) =>
            !lastPaidDate.HasValue ? (nextDate >= effectiveDate) : (nextDate > lastPaidDate && nextDate > lastCashflowDate);
    }
}
