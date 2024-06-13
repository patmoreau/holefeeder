// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using DrifterApps.Seeds.Application;

using Holefeeder.Application.Context;
using Holefeeder.Application.UserContext;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;

using Microsoft.EntityFrameworkCore;

namespace Holefeeder.Application.Features.Statistics.Queries;

public partial class GetSummary
{
    internal class Handler(IUserContext userContext, BudgetingContext context) : IRequestHandler<Request, SummaryDto>
    {
        public async Task<SummaryDto> Handle(Request request, CancellationToken cancellationToken)
        {
            var (intervalType, frequency) = DateIntervalType.GetIntervalTypeFromRange(request.From, request.To);
            var query = from category in context.Categories
                        join transaction in context.Transactions on category.Id equals transaction.CategoryId
                        where category.UserId == userContext.Id
                        group transaction by new
                        {
                            category.Type,
                            transaction.Date
                        }
                into groupedTransactions
                        select new
                        {
                            groupedTransactions.Key.Type,
                            groupedTransactions.Key.Date,
                            TotalAmount = groupedTransactions.Sum(t => t.Amount)
                        };

            var results = await query.ToListAsync(cancellationToken);

            var gains = results
                .Where(x => x.Type == CategoryType.Gain)
                .GroupBy(x => intervalType.Interval(request.From, x.Date, frequency))
                .Select(x => (x.Key, Value: x.Sum(y => y.TotalAmount)))
                .ToDictionary();
            var expenses = results
                .Where(x => x.Type == CategoryType.Expense)
                .GroupBy(x => intervalType.Interval(request.From, x.Date, frequency))
                .Select(x => (x.Key, Value: x.Sum(y => y.TotalAmount)))
                .ToDictionary();

            return new SummaryDto(
                new SummaryValue(Period(gains, intervalType.AddIteration(request.From, -frequency)),
                    Period(expenses, intervalType.AddIteration(request.From, -frequency))),
                new SummaryValue(Period(gains, request.From), Period(expenses, request.From)),
                new SummaryValue(Average(gains), Average(expenses)));
        }

        private static decimal Period(IDictionary<(DateOnly From, DateOnly To), decimal> dto, DateOnly asOf) =>
            dto.FirstOrDefault(x => x.Key.From == asOf).Value;

        private static decimal Average(IDictionary<(DateOnly From, DateOnly To), decimal> dto) =>
            Math.Round(dto.Sum(x => x.Value) / dto.Count, 2);
    }
}
