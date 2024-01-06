// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Domain.Features.Categories;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetSummary
{
    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, SummaryDto>
    {
        public async Task<SummaryDto> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = from category in context.Categories
                        join transaction in context.Transactions on category.Id equals transaction.CategoryId
                        where category.UserId == userContext.Id && !category.System
                        group transaction by new
                        {
                            category.Type,
                            transaction.Date.Year,
                            transaction.Date.Month
                        }
                into groupedTransactions
                        select new
                        {
                            groupedTransactions.Key.Type,
                            groupedTransactions.Key.Year,
                            groupedTransactions.Key.Month,
                            TotalAmount = groupedTransactions.Sum(t => t.Amount)
                        }
                into summarizedTransactions
                        group summarizedTransactions by new
                        {
                            summarizedTransactions.Type,
                            summarizedTransactions.Year
                        }
                into groupedSummaries
                        select new
                        {
                            groupedSummaries.Key.Type,
                            groupedSummaries.Key.Year,
                            TotalAmountByYear = groupedSummaries.Sum(s => s.TotalAmount),
                            MonthlyTotals = groupedSummaries.Select(s => new { s.Month, TotalAmountByMonth = s.TotalAmount })
                        };

            var results = await query.ToListAsync(cancellationToken);

            var gains = results
                .Where(x => x.Type == CategoryType.Gain)
                .GroupBy(x => x.Type,
                    arg => new YearStatisticsDto(arg.Year, arg.TotalAmountByYear,
                        arg.MonthlyTotals.Select(x => new MonthStatisticsDto(x.Month, x.TotalAmountByMonth)).ToList()))
                .ToList();
            var expenses = results
                .Where(x => x.Type == CategoryType.Expense).GroupBy(x => x.Type,
                    arg => new YearStatisticsDto(arg.Year, arg.TotalAmountByYear,
                        arg.MonthlyTotals.Select(x => new MonthStatisticsDto(x.Month, x.TotalAmountByMonth)).ToList()))
                .ToList();

            return new SummaryDto(
                new SummaryValue(Month(gains, request.AsOf.AddMonths(-1)), Month(expenses, request.AsOf.AddMonths(-1))),
                new SummaryValue(Month(gains, request.AsOf), Month(expenses, request.AsOf)),
                new SummaryValue(Average(gains), Average(expenses)));
        }

        private static decimal Month(IList<IGrouping<CategoryType, YearStatisticsDto>>? dto, DateOnly asOf) =>
            dto == null
                ? 0m
                : dto.SelectMany(x =>
                        x.Where(y => y.Year == asOf.Year).SelectMany(y => y.Months.Where(z => z.Month == asOf.Month)))
                    .Sum(x => x.Total);

        private static decimal Average(IList<IGrouping<CategoryType, YearStatisticsDto>>? dto) =>
            dto == null
                ? 0m
                : Math.Round(dto.Sum(x => x.Sum(y => y.Total)) / dto.Sum(x => x.Sum(y => y.Months.Count())), 2);
    }
}
