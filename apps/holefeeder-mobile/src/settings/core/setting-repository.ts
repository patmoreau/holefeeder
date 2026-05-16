import { DataMetrics } from '@/settings/core/data-metrics';
import type { AsyncResult } from '@/shared/core/result';

export type SettingRepository = {
  watchDataMetrics: (onDataChange: (result: AsyncResult<DataMetrics>) => void) => () => void;
};
