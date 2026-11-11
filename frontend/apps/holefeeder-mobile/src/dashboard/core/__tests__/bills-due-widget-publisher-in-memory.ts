import { Result } from '@holefeeder/shared/core';
import { BillsDueEntry } from '@/dashboard/core/publish-bills-due-widget/bills-due-summary';
import { BillsDueWidgetPublisher } from '@/dashboard/core/publish-bills-due-widget/bills-due-widget-publisher';

export type BillsDueWidgetPublisherInMemory = BillsDueWidgetPublisher & {
  isFailing: (errors: string[]) => void;
  published: () => BillsDueEntry[][];
  clears: () => number;
};

export const BillsDueWidgetPublisherInMemory = (): BillsDueWidgetPublisherInMemory => {
  const publishedInMemory: BillsDueEntry[][] = [];
  let clearsInMemory = 0;
  let errorsInMemory: string[] = [];

  const publish = async (entries: BillsDueEntry[]): Promise<Result<void>> => {
    if (errorsInMemory.length > 0) return Result.failure(errorsInMemory);
    publishedInMemory.push(entries);
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
