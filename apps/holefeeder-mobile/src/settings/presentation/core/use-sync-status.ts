import { useStatus } from '@powersync/react';

export interface SyncStatusInfo {
  connected: boolean;
  lastSyncedAt: Date | null;
  dataFlowStatus: {
    downloading: boolean;
    uploading: boolean;
  };
}

export function useSyncStatus(): SyncStatusInfo {
  const status = useStatus();

  return {
    connected: status.connected ?? false,
    lastSyncedAt: status?.lastSyncedAt ?? null,
    dataFlowStatus: status?.dataFlowStatus
      ? { downloading: status.dataFlowStatus.downloading ?? false, uploading: status.dataFlowStatus.uploading ?? false }
      : { downloading: false, uploading: false },
  };
}
