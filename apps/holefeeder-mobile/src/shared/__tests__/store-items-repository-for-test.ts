import { type AsyncResult, Result } from '@/shared/core/result';
import { StoreItem } from '@/shared/core/store-item';
import { StoreItemsRepository, StoreItemsRepositoryErrors } from '@/shared/core/store-items-repository';

export type StoreItemsRepositoryInMemory = StoreItemsRepository & {
  add: (...items: StoreItem[]) => void;
  isLoading: () => void;
  isFailing: (errors: string[]) => void;
};

export const StoreItemsRepositoryInMemory = (): StoreItemsRepositoryInMemory => {
  const itemsInMemory: StoreItem[] = [];
  let loadingInMemory = false;
  let errorsInMemory: string[] = [];

  const watchForCode = (code: string, onDataChange: (result: AsyncResult<StoreItem>) => void) => {
    const item = itemsInMemory.find((item) => item.code === code);
    if (loadingInMemory) {
      onDataChange(Result.loading());
    } else if (errorsInMemory.length > 0) {
      onDataChange(Result.failure(errorsInMemory));
    } else if (item) {
      onDataChange(Result.success(item));
    } else {
      onDataChange(Result.failure([StoreItemsRepositoryErrors.storeItemNotFound]));
    }
    // Return unsubscribe function
    return () => {};
  };

  const getByCode = async (code: string) => {
    const item = itemsInMemory.find((item) => item.code === code);
    if (item) {
      return Result.success(item);
    }
    return Result.failure([StoreItemsRepositoryErrors.storeItemNotFound]);
  };

  const save = (storeItem: StoreItem) => {
    const index = itemsInMemory.findIndex((item) => item.code === storeItem.code);
    if (index >= 0) {
      itemsInMemory[index] = storeItem;
    } else {
      itemsInMemory.push(storeItem);
    }
    return Promise.resolve(Result.success(undefined));
  };

  const add = (...items: StoreItem[]) => itemsInMemory.push(...items);

  const isLoading = () => (loadingInMemory = true);

  const isFailing = (errors: string[]) => (errorsInMemory = errors);

  return { watchForCode: watchForCode, getByCode: getByCode, save: save, add: add, isLoading: isLoading, isFailing: isFailing };
};
