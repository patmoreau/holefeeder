import { DataMetrics } from '@/settings/core/data-metrics';
import { aCount } from '@/shared/__tests__/number-for-test';

const defaultDataMetricsForTest = (): DataMetrics => ({
  accounts: aCount(),
  cashflows: aCount(),
  categories: aCount(),
  storeItems: aCount(),
  transactions: aCount(),
  outstandingTransactions: aCount(),
});

export const aDataMetrics = (overrides?: Partial<DataMetrics>): DataMetrics => ({
  ...defaultDataMetricsForTest(),
  ...overrides,
});
