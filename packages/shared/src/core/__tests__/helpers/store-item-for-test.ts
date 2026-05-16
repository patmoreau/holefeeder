import { StoreItem } from '../../store-item';
import { anId, aWord } from './string-for-test';

const defaultStoreItem = (): StoreItem => ({
  id: anId(),
  code: aWord(),
  data: '{ "amount": 1000, "description": "test" }',
});

export const aStoreItem = (overrides?: Partial<StoreItem>): StoreItem => ({
  ...defaultStoreItem(),
  ...overrides,
});

export const toStoreItem = (storeItem: StoreItem): StoreItem => storeItem;
