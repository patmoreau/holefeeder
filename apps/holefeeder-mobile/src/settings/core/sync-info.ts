import { DataMetrics } from '@/settings/core/data-metrics';

export type SyncInfo = {
  connected: boolean;
  lastSyncedAt?: Date;
  dataFlowStatus: {
    downloading: boolean;
    uploading: boolean;
  };
  dataMetrics: DataMetrics;
};

export const DEFAULT_SYNC_INFO: SyncInfo = {
  connected: false,
  lastSyncedAt: undefined,
  dataFlowStatus: {
    downloading: false,
    uploading: false,
  },
  dataMetrics: {
    accounts: 0,
    cashflows: 0,
    categories: 0,
    storeItems: 0,
    transactions: 0,
    outstandingTransactions: 0,
  },
};
