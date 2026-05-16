import { AbstractPowerSyncDatabase } from '@powersync/common';
import { DataMetrics, DEFAULT_DATA_METRICS } from '@/settings/core/data-metrics';
import { SettingRepository } from '@/settings/core/setting-repository';
import { AsyncResult, Result } from '@/shared/core/result';
import { watchSingle } from '@/shared/persistence/watch-query';

type DataMetricsRow = {
  accounts: number;
  cashflows: number;
  categories: number;
  storeItems: number;
  transactions: number;
  outstandingTransactions: number;
};

export const SettingRepositoryInPowersync = (db: AbstractPowerSyncDatabase): SettingRepository => {
  const watchDataMetrics = (onDataChange: (result: AsyncResult<DataMetrics>) => void) =>
    watchSingle<DataMetricsRow, DataMetrics>(
      db,
      `
        SELECT
            (SELECT count(*) FROM accounts) as accounts,
            (SELECT count(*) FROM cashflows) as cashflows,
            (SELECT count(*) FROM categories) as categories,
            (SELECT count(*) FROM store_items) as storeItems,
            (SELECT count(*) FROM transactions) as transactions,
            (SELECT count(*) FROM ps_crud) as outstandingTransactions
            `,
      [],
      (row) => ({
        accounts: row.accounts,
        cashflows: row.cashflows,
        categories: row.categories,
        storeItems: row.storeItems,
        transactions: row.transactions,
        outstandingTransactions: row.outstandingTransactions,
      }),
      onDataChange,
      () => Result.success(DEFAULT_DATA_METRICS),
      'watchDataMetrics'
    );

  return {
    watchDataMetrics: watchDataMetrics,
  };
};
