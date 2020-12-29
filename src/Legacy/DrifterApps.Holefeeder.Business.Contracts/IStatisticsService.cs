using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business
{
    public interface IStatisticsService
    {
        Task<IEnumerable<StatisticsEntity<CategoryInfoEntity>>> StatisticsAsync(Guid userId, DateTime effectiveDate, DateIntervalType intervalType, int frequency, CancellationToken cancellationToken = default);
        Task<IEnumerable<StatisticsEntity<string>>> StatisticsByTagsAsync(Guid userId, string categoryId, DateTime effectiveDate, DateIntervalType intervalType, int frequency, CancellationToken cancellationToken = default);
    }
}
