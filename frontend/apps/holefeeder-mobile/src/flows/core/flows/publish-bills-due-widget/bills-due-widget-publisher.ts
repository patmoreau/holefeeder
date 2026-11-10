import { Result } from '@holefeeder/shared/core';
import { BillsDueEntry } from './bills-due-summary';

export const BillsDueWidgetPublisherErrors = {
  publishFailed: 'publish-bills-due-widget-failed',
};

export type BillsDueWidgetPublisher = {
  publish: (entries: BillsDueEntry[]) => Promise<Result<void>>;
  clear: () => Promise<Result<void>>;
};
