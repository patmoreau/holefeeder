import { useEffect, useMemo, useState } from 'react';
import { DataMetrics } from '@/settings/core/data-metrics';
import { SyncInfo } from '@/settings/core/sync-info';
import { WatchDataMetricsUseCase } from '@/settings/core/watch-data-metrics/watch-data-metrics-use-case';
import { useSyncStatus } from '@/settings/presentation/core/use-sync-status';
import { AsyncResult, Result } from '@/shared/core/result';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useSyncInfo = (): AsyncResult<SyncInfo> => {
  const { settingRepository } = useRepositories();
  const { connected, lastSyncedAt, dataFlowStatus } = useSyncStatus();
  const [dataMetrics, setDataMetrics] = useState<AsyncResult<DataMetrics>>(Result.loading());

  const useCase = useMemo(() => WatchDataMetricsUseCase(settingRepository), [settingRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watch(setDataMetrics);
    return () => unsubscribe();
  }, [useCase]);

  if (dataMetrics.isLoading) return Result.loading();
  if (dataMetrics.isFailure) return Result.failure(dataMetrics.errors);

  return Result.success<SyncInfo>({
    connected: connected,
    lastSyncedAt: lastSyncedAt ? lastSyncedAt : undefined,
    dataFlowStatus: dataFlowStatus,
    dataMetrics: dataMetrics.value,
  });
};
