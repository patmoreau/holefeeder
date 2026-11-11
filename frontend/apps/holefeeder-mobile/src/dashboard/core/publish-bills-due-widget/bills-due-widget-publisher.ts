import { Result } from '@holefeeder/shared/core';
import { BillsDueEntry, BillsDuePeriod } from './bills-due-summary';

export const BillsDueWidgetPublisherErrors = {
  publishFailed: 'publish-bills-due-widget-failed',
};

export type BillsDueWidgetPublisher = {
  publish: (entries: BillsDueEntry[], period: BillsDuePeriod) => Promise<Result<void>>;
  clear: () => Promise<Result<void>>;
};
