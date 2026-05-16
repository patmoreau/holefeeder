import { type AsyncResult, Result } from './result';
import { StoreItem } from './store-item';

export type StoreItemsRepository = {
  watchForCode: (code: string, onDataChange: (result: AsyncResult<StoreItem>) => void) => () => void;
  getByCode: (code: string) => Promise<Result<StoreItem>>;
  save: (storeItem: StoreItem) => Promise<Result<void>>;
};

export const StoreItemsRepositoryErrors = {
  storeItemNotFound: 'store-item-not-found',
  saveStoreItemFailed: 'save-store-item-failed',
};
