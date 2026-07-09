import type { AsyncResult } from '@holefeeder/shared/core';
import { DataMetrics } from '@/settings/core/data-metrics';

export type SettingRepository = {
  watchDataMetrics: (onDataChange: (result: AsyncResult<DataMetrics>) => void) => () => void;
};
