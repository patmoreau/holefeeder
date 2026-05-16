import { DataMetrics } from '@/settings/core/data-metrics';
import { SettingRepository } from '@/settings/core/setting-repository';
import { type AsyncResult } from '@/shared/core/result';

export const WatchDataMetricsUseCase = (settingsRepository: SettingRepository) => {
  const watch = (onDataChange: (result: AsyncResult<DataMetrics>) => void) =>
    settingsRepository.watchDataMetrics((result) => onDataChange(result));

  return {
    watch: watch,
  };
};
