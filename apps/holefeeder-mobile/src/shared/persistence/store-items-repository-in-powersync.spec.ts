import { waitFor } from '@testing-library/react-native';
import { aStoreItem, toStoreItem } from '@/shared/__tests__/store-item-for-test';
import { aWord } from '@/shared/__tests__/string-for-test';
import { type AsyncResult } from '@/shared/core/result';
import { StoreItemsRepositoryErrors } from '@/shared/core/store-items-repository';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { StoreItemsRepositoryInPowersync } from '@/shared/persistence/store-items-repository-in-powersync';
import { WatchQueryErrors } from '@/shared/persistence/watch-query';

describe('StoreItemsRepositoryInPowersync', () => {
  let db: DatabaseForTest;

  beforeEach(async () => {
    db = await setupDatabaseForTest();
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  describe('watchForCode', () => {
    it('retrieves a stored store item', async () => {
      const storeItem = await aStoreItem().store(db);
      const repo = StoreItemsRepositoryInPowersync(db);

      let result: AsyncResult<any> | undefined;
      const unsubscribe = repo.watchForCode(storeItem.code, (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue(toStoreItem(storeItem));

      unsubscribe();
    });

    it('returns not found when no store items exist', async () => {
      const repo = StoreItemsRepositoryInPowersync(db);

      let result: AsyncResult<any> | undefined;
      const unsubscribe = repo.watchForCode(aWord(), (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors([WatchQueryErrors.rowNotFound]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      const repo = StoreItemsRepositoryInPowersync(db);

      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<any> | undefined;
      const unsubscribe = repo.watchForCode(aWord(), (data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('getByCode', () => {
    it('retrieves a stored store item', async () => {
      const storeItem = await aStoreItem().store(db);
      const repo = StoreItemsRepositoryInPowersync(db);

      const result = await repo.getByCode(storeItem.code);

      expect(result).toBeSuccessWithValue({
        id: storeItem.id,
        code: storeItem.code,
        data: storeItem.data,
      });
    });

    it('returns not found when code does not exist', async () => {
      const repo = StoreItemsRepositoryInPowersync(db);

      const result = await repo.getByCode(aWord());

      expect(result).toBeFailureWithErrors([StoreItemsRepositoryErrors.storeItemNotFound]);
    });

    it('handles database errors', async () => {
      const repo = StoreItemsRepositoryInPowersync(db);

      await db.close();

      const result = await repo.getByCode('some-code');

      expect(result).toBeFailureWithErrors([StoreItemsRepositoryErrors.saveStoreItemFailed]);
    });
  });

  describe('save', () => {
    it('saves a new store item', async () => {
      const storeItem = aStoreItem();
      const repo = StoreItemsRepositoryInPowersync(db);

      const result = await repo.save(toStoreItem(storeItem));

      expect(result).toBeSuccessWithValue(undefined);

      const storedItem = await db.get('SELECT id, code, data FROM store_items WHERE code = ?', [storeItem.code]);
      expect(storedItem).toBeDefined();
      expect(storedItem).toEqual(toStoreItem(storeItem));
    });
  });
});
