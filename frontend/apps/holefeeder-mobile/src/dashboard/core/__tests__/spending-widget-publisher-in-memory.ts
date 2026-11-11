import { Result } from '@holefeeder/shared/core';
import { SpendingSnapshot } from '@/dashboard/core/publish-spending-widget/spending-snapshot';
import { SpendingWidgetPublisher } from '@/dashboard/core/publish-spending-widget/spending-widget-publisher';

export type SpendingWidgetPublisherInMemory = SpendingWidgetPublisher & {
  isFailing: (errors: string[]) => void;
  published: () => SpendingSnapshot[];
  clears: () => number;
};

export const SpendingWidgetPublisherInMemory = (): SpendingWidgetPublisherInMemory => {
  const publishedInMemory: SpendingSnapshot[] = [];
  let clearsInMemory = 0;
  let errorsInMemory: string[] = [];

  const publish = async (snapshot: SpendingSnapshot): Promise<Result<void>> => {
    if (errorsInMemory.length > 0) return Result.failure(errorsInMemory);
    publishedInMemory.push(snapshot);
    return Result.success();
  };

  const clear = async (): Promise<Result<void>> => {
    if (errorsInMemory.length > 0) return Result.failure(errorsInMemory);
    clearsInMemory++;
    return Result.success();
  };

  return {
    publish: publish,
    clear: clear,
    isFailing: (errors: string[]) => (errorsInMemory = errors),
    published: () => [...publishedInMemory],
    clears: () => clearsInMemory,
  };
};
