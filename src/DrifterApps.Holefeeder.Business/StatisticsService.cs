using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common;
using DrifterApps.Holefeeder.Common.Enums;
using DrifterApps.Holefeeder.Common.Extensions;

namespace DrifterApps.Holefeeder.Business
{
    public class StatisticsService : IStatisticsService
    {
        static readonly string[] noTag = new[] { "<no tag>" };

        private readonly ITransactionsService _transactionsService;
        private readonly ICategoriesService _categoriesService;
        private readonly IMapper _mapper;

        public StatisticsService(ITransactionsService transactionsService, ICategoriesService categoriesService, IMapper mapper)
        {
            _transactionsService = transactionsService.ThrowIfNull(nameof(transactionsService));
            _categoriesService = categoriesService.ThrowIfNull(nameof(categoriesService));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        public Task<int> CountAsync(string userId, QueryParams query, CancellationToken cancellationToken = default) => _transactionsService.CountAsync(userId, query, cancellationToken);

        public Task<IEnumerable<TransactionDetailEntity>> FindWithDetailsAsync(string userId, QueryParams query, CancellationToken cancellationToken = default) => _transactionsService.FindWithDetailsAsync(userId, query, cancellationToken);

        public async Task<IEnumerable<StatisticsEntity<CategoryInfoEntity>>> StatisticsAsync(string userId, DateTime effectiveDate, DateIntervalType intervalType, int frequency, CancellationToken cancellationToken = default)
        {
            var transactions = await _transactionsService.FindAsync(userId, new QueryParams(null, null, new HashSet<string>(new[] { "date" }), null), cancellationToken).ConfigureAwait(false);
            var categories = (await _categoriesService.FindAsync(userId, null, cancellationToken).ConfigureAwait(false)).Where(c => !c.System).Select(_mapper.Map<CategoryInfoEntity>);

            var yearly = transactions
                            .GroupBy(t => (
                                CategoryId: t.Category,
                                From: new DateTime(t.Date.Year, 1, 1),
                                To: new DateTime(t.Date.Year, 1, 1).AddYears(1).AddDays(-1)
                            ))
                            .Select(g => (
                                CategoryId: g.Key.CategoryId,
                                From: g.Key.From,
                                To: g.Key.To,
                                Count: g.Count(),
                                Amount: g.Sum(t => t.Amount)
                            ))
                            .GroupBy(p => p.CategoryId)
                            .Select(p => (
                                CategoryId: p.Key,
                                Data: new List<SeriesEntity>(p.Select(s => new SeriesEntity(s.From, s.To, s.Count, s.Amount)))
                            ))
                            .ToList();

            var monthly = transactions
                            .GroupBy(t => (
                                CategoryId: t.Category,
                                From: new DateTime(t.Date.Year, t.Date.Month, 1),
                                To: new DateTime(t.Date.Year, t.Date.Month, 1).AddMonths(1).AddDays(-1)
                            ))
                            .Select(g => (
                                CategoryId: g.Key.CategoryId,
                                From: g.Key.From,
                                To: g.Key.To,
                                Count: g.Count(),
                                Amount: g.Sum(t => t.Amount)
                            ))
                            .GroupBy(p => p.CategoryId)
                            .Select(p => (
                                CategoryId: p.Key,
                                Data: new List<SeriesEntity>(p.Select(s => new SeriesEntity(s.From, s.To, s.Count, s.Amount)))
                            ))
                            .ToList();

            var periods = transactions
                            .Select(t => (
                                CategoryId: t.Category,
                                Period: t.Date.Interval(effectiveDate, intervalType, frequency),
                                Transaction: t
                            ))
                            .GroupBy(t => (
                                CategoryId: t.CategoryId,
                                From: t.Period.from,
                                To: t.Period.to
                            ))
                            .Select(g => (
                                CategoryId: g.Key.CategoryId,
                                From: g.Key.From,
                                To: g.Key.To,
                                Count: g.Count(),
                                Amount: g.Sum(t => t.Transaction.Amount)
                            ))
                            .GroupBy(p => p.CategoryId)
                            .Select(p => (
                                CategoryId: p.Key,
                                Data: new List<SeriesEntity>(p.Select(s => new SeriesEntity(s.From, s.To, s.Count, s.Amount)))
                            ))
                            .ToList();

            var statistics =
                from y in yearly
                join m in monthly on y.CategoryId equals m.CategoryId
                join p in periods on y.CategoryId equals p.CategoryId
                join c in categories on y.CategoryId equals c.Id
                select new StatisticsEntity<CategoryInfoEntity>(c, y.Data, m.Data, p.Data);

            return statistics;
        }

        public async Task<IEnumerable<StatisticsEntity<string>>> StatisticsByTagsAsync(string userId, string categoryId, DateTime effectiveDate, DateIntervalType intervalType, int frequency, CancellationToken cancellationToken = default)
        {
            var transactions = await _transactionsService.FindAsync(userId, new QueryParams(null, null, new[] { "date" }, new[] { $"category={categoryId}" }), cancellationToken).ConfigureAwait(false);

            var yearly = transactions
                            .SelectMany(t => (t.Tags?.Any() ?? false) ? t.Tags : noTag, (transaction, tag) => (transaction, tag))
                            .GroupBy(t => (
                                Tag: t.tag.Trim(),
                                From: new DateTime(t.transaction.Date.Year, 1, 1),
                                To: new DateTime(t.transaction.Date.Year, 1, 1).AddYears(1).AddDays(-1)
                            ))
                            .Select(g => (
                                Tag: g.Key.Tag,
                                From: g.Key.From,
                                To: g.Key.To,
                                Count: g.Count(),
                                Amount: g.Sum(t => t.transaction.Amount)
                            ))
                            .GroupBy(p => p.Tag)
                            .Select(p => (
                                Tag: p.Key,
                                Data: new List<SeriesEntity>(p.Select(s => new SeriesEntity(s.From, s.To, s.Count, s.Amount)))
                            ))
                            .ToList();

            var monthly = transactions
                            .SelectMany(t => (t.Tags?.Any() ?? false) ? t.Tags : noTag, (transaction, tag) => (transaction, tag))
                            .GroupBy(t => (
                                Tag: t.tag.Trim(),
                                From: new DateTime(t.transaction.Date.Year, t.transaction.Date.Month, 1),
                                To: new DateTime(t.transaction.Date.Year, t.transaction.Date.Month, 1).AddMonths(1).AddDays(-1)
                            ))
                            .Select(g => (
                                Tag: g.Key.Tag,
                                From: g.Key.From,
                                To: g.Key.To,
                                Count: g.Count(),
                                Amount: g.Sum(t => t.transaction.Amount)
                            ))
                            .GroupBy(p => p.Tag)
                            .Select(p => (
                                Tag: p.Key,
                                Data: new List<SeriesEntity>(p.Select(s => new SeriesEntity(s.From, s.To, s.Count, s.Amount)))
                            ))
                            .ToList();

            var periods = transactions
                            .SelectMany(t => (t.Tags?.Any() ?? false) ? t.Tags : noTag, (transaction, tag) => (transaction, tag))
                            .Select(t => (
                                Tag: t.tag.Trim(),
                                Period: t.transaction.Date.Interval(effectiveDate, intervalType, frequency),
                                Transaction: t
                            ))
                            .GroupBy(t => (
                                Tag: t.Tag,
                                From: t.Period.from,
                                To: t.Period.to
                            ))
                            .Select(g => (
                                Tag: g.Key.Tag,
                                From: g.Key.From,
                                To: g.Key.To,
                                Count: g.Count(),
                                Amount: g.Sum(t => t.Transaction.transaction.Amount)
                            ))
                            .GroupBy(p => p.Tag)
                            .Select(p => (
                                Tag: p.Key,
                                Data: new List<SeriesEntity>(p.Select(s => new SeriesEntity(s.From, s.To, s.Count, s.Amount)))
                            ))
                            .ToList();

            var statistics =
                from y in yearly
                join m in monthly on y.Tag equals m.Tag
                join p in periods on y.Tag equals p.Tag
                select new StatisticsEntity<string>(y.Tag, y.Data, m.Data, p.Data);

            return statistics;
        }
    }
}