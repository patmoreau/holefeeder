using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DrifterApps.Holefeeder.Business.Entities;
using DrifterApps.Holefeeder.Common.Enums;

namespace DrifterApps.Holefeeder.Business
{
    public interface IStatisticsService
    {
        Task<IEnumerable<StatisticsEntity<CategoryInfoEntity>>> StatisticsAsync(string userId, DateTime effectiveDate, DateIntervalType intervalType, int frequency);
        Task<IEnumerable<StatisticsEntity<string>>> StatisticsByTagsAsync(string userId, string categoryId, DateTime effectiveDate, DateIntervalType intervalType, int frequency);
    }
}