import { aStoreItem, toStoreItem } from '@/shared/__tests__/store-item-for-test';
import { Id, IdErrors } from '@/shared/core/id';
import { StoreItem, StoreItemErrors } from './store-item';

describe('StoreItem', () => {
  it('creates a store item', () => {
    const storeItem = aStoreItem();
    const result = StoreItem.create(storeItem);
    expect(result).toBeSuccessWithValue(toStoreItem(storeItem));
  });

  it('fails when Id is invalid', () => {
    const storeItem = aStoreItem({ id: Id.valid('invalid') });
    const result = StoreItem.create(storeItem);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('fails when Code is invalid', () => {
    const storeItem = aStoreItem({ code: '' });
    const result = StoreItem.create(storeItem);
    expect(result).toBeFailureWithErrors([StoreItemErrors.invalidCode]);
  });

  it('fails when Data is not json', () => {
    const storeItem = aStoreItem({ data: 'invalid' });
    const result = StoreItem.create(storeItem);
    expect(result).toBeFailureWithErrors([StoreItemErrors.invalidData]);
  });
});
