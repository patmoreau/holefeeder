import type { AsyncResult } from '@holefeeder/shared/core';
import { Settings } from '@holefeeder/shared/core';
import { SummaryData } from './summary-data';

export type SummaryRepository = {
  watch: (onDataChange: (result: AsyncResult<SummaryData[]>) => void, settings: Settings) => () => void;
};
