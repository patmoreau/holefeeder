import { Settings } from '@/settings/core/settings';
import { type AsyncResult } from '@/shared/core/result';
import { SummaryData } from './summary-data';

export type DashboardRepository = {
  watch: (onDataChange: (result: AsyncResult<SummaryData[]>) => void, settings: Settings) => () => void;
};
