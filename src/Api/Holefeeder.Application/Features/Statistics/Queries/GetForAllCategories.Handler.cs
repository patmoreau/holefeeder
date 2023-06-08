// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DrifterApps.Seeds.Application;
using Holefeeder.Application.Context;
using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetForAllCategories
{
    internal class Handler : IRequestHandler<Request, IEnumerable<StatisticsDto>>
    {
        private readonly BudgetingContext _context;
        private readonly IUserContext _userContext;

        public Handler(IUserContext userContext, BudgetingContext context)
        {
            _userContext = userContext;
            _context = context;
        }

        public async Task<IEnumerable<StatisticsDto>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = from category in _context.Categories
                        join transaction in _context.Transactions on category.Id equals transaction.CategoryId
                        where category.UserId == _userContext.UserId && !category.System
                        group transaction by new
                        {
                            category.Id,
                            category.Name,
                            category.Type,
                            category.Color,
                            transaction.Date.Year,
                            transaction.Date.Month
                        }
                into groupedTransactions
                        select new
                        {
                            groupedTransactions.Key.Id,
                            groupedTransactions.Key.Name,
                            groupedTransactions.Key.Type,
                            groupedTransactions.Key.Color,
                            groupedTransactions.Key.Year,
                            groupedTransactions.Key.Month,
                            TotalAmount = groupedTransactions.Sum(t => t.Amount)
                        }
                into summarizedTransactions
                        group summarizedTransactions by new
                        {
                            summarizedTransactions.Id,
                            summarizedTransactions.Name,
                            summarizedTransactions.Type,
                            summarizedTransactions.Color,
                            summarizedTransactions.Year
                        }
                into groupedSummaries
                        select new
                        {
                            groupedSummaries.Key.Id,
                            groupedSummaries.Key.Name,
                            groupedSummaries.Key.Type,
                            groupedSummaries.Key.Color,
                            groupedSummaries.Key.Year,
                            TotalAmountByYear = groupedSummaries.Sum(s => s.TotalAmount),
                            MonthlyTotals = groupedSummaries.Select(s => new { s.Month, TotalAmountByMonth = s.TotalAmount })
                        };

            var results = await query.ToListAsync(cancellationToken: cancellationToken);

            var dto = results
                .Where(x => x.Type.IsExpense)
                .GroupBy(x => new { x.Id, x.Name, x.Color },
                    arg => new YearStatisticsDto(arg.Year, arg.TotalAmountByYear,
                        arg.MonthlyTotals.Select(x => new MonthStatisticsDto(x.Month, x.TotalAmountByMonth)).ToList()))
                .OrderBy(x => x.Key.Name);

            return dto.Select(grouping => new StatisticsDto(grouping.Key.Id, grouping.Key.Name, grouping.Key.Color,
                Math.Round(grouping.Sum(x => x.Total) / grouping.Sum(x => x.Months.Count()), 2), grouping));
        }
    }
}
