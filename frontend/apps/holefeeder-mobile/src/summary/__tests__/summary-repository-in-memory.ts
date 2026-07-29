import { type AsyncResult, Result } from '@holefeeder/shared/core';
import { SummaryData } from '@/summary/core/summary-data';
import { SummaryRepository } from '@/summary/core/summary-repository';

export type SummaryRepositoryInMemory = SummaryRepository & {
  add: (...items: SummaryData[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const SummaryRepositoryInMemory = (): SummaryRepositoryInMemory => {
  const itemsInMemory: SummaryData[] = [];
  let loadingInMemory = false;
  let errorsInMemory: string[] = [];

  const watch = (onDataChange: (result: AsyncResult<SummaryData[]>) => void) => {
    if (loadingInMemory) {
      onDataChange(Result.loading());
    } else if (errorsInMemory.length > 0) {
      onDataChange(Result.failure(errorsInMemory));
    } else {
      onDataChange(Result.success(itemsInMemory));
    }
    // Return unsubscribe function
    return () => {};
  };

  const add = (...items: SummaryData[]) => itemsInMemory.push(...items);

  const isLoading = () => (loadingInMemory = true);

  const isFailing = (errors: string[]) => (errorsInMemory = errors);

  return { watch: watch, add: add, isLoading: isLoading, isFailing: isFailing };
};
