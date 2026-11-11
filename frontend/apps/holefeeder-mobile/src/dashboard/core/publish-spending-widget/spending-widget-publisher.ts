import { Result } from '@holefeeder/shared/core';
import { SpendingSnapshot } from './spending-snapshot';

export const SpendingWidgetPublisherErrors = {
  publishFailed: 'publish-spending-widget-failed',
};

export type SpendingWidgetPublisher = {
  publish: (snapshot: SpendingSnapshot) => Promise<Result<void>>;
  clear: () => Promise<Result<void>>;
};
