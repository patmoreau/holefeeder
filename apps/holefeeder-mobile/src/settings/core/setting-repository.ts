import type { AsyncResult } from '@holefeeder/core';
import { DataMetrics } from '@/settings/core/data-metrics';

export type SettingRepository = {
  watchDataMetrics: (onDataChange: (result: AsyncResult<DataMetrics>) => void) => () => void;
};
