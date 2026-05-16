import type { AsyncResult } from '@holefeeder/core';
import { Settings } from '@/settings/core/settings';
import { SummaryData } from './summary-data';

export type DashboardRepository = {
  watch: (onDataChange: (result: AsyncResult<SummaryData[]>) => void, settings: Settings) => () => void;
};
