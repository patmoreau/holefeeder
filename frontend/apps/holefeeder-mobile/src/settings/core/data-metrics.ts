export type DataMetrics = {
  accounts: number;
  cashflows: number;
  categories: number;
  storeItems: number;
  transactions: number;
  outstandingTransactions: number;
};

export const DEFAULT_DATA_METRICS: DataMetrics = {
  accounts: 0,
  cashflows: 0,
  categories: 0,
  storeItems: 0,
  transactions: 0,
  outstandingTransactions: 0,
};
