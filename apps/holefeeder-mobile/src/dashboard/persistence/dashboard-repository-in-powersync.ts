import { AbstractPowerSyncDatabase } from '@powersync/common';
import { DashboardRepository } from '@/dashboard/core/dashboard-repository';
import { SummaryData } from '@/dashboard/core/summary-data';
import { Settings } from '@/settings/core/settings';
import { DateIntervalTypes } from '@/shared/core/date-interval-type';
import { Money } from '@/shared/core/money';
import { type AsyncResult } from '@/shared/core/result';
import { watchQuery } from '@/shared/persistence/watch-query';

type SummaryDataRow = {
  type: string;
  bucket_date: string;
  total: number;
};

export const DashboardRepositoryInPowersync = (db: AbstractPowerSyncDatabase): DashboardRepository => {
  const getBucketLogic = (settings: Settings) => {
    const { effectiveDate, intervalType, frequency } = settings;

    if (intervalType === DateIntervalTypes.weekly) {
      const days = frequency * 7;
      // Calculate how many 'days' intervals have passed since the anchor date
      return `CAST((julianday(t.date) - julianday('${effectiveDate}')) / ${days} AS INTEGER)`;
    }

    if (intervalType === DateIntervalTypes.monthly) {
      // Calculate total months since anchor, divided by frequency
      return `CAST(((strftime('%Y', t.date) - strftime('%Y', '${effectiveDate}')) * 12 + 
            (strftime('%m', t.date) - strftime('%m', '${effectiveDate}'))) / ${frequency} AS INTEGER)`;
    }

    return 't.date'; // Fallback
  };

  const watch = (onDataChange: (result: AsyncResult<SummaryData[]>) => void, settings: Settings) =>
    watchQuery<SummaryDataRow, SummaryData>(
      db,
      `
        SELECT
          c.type,
          MIN(t.date) as bucket_date,
          SUM(t.amount) as total
        FROM transactions t
               INNER JOIN (
          SELECT id, type
          FROM categories
          WHERE system = 0
        ) c ON c.id = t.category_id
        GROUP BY c.type, ${getBucketLogic(settings)}
        `,
      [],
      (row) =>
        SummaryData.valid({
          type: row.type,
          date: row.bucket_date,
          total: Money.fromCents(row.total),
        }),
      onDataChange,
      'watchDashboard'
    );

  return {
    watch: watch,
  };
};
