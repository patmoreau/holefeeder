import { waitFor } from '@testing-library/react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { DataMetrics } from '@/settings/core/data-metrics';
import { SettingRepositoryInPowersync } from '@/settings/persistence/setting-repository-in-powersync';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { type AsyncResult } from '@/shared/core/result';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';

describe('SettingRepositoryInPowersync', () => {
  let db: DatabaseForTest;

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    await db.execute('CREATE TABLE ps_crud (id INTEGER PRIMARY KEY);');
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  describe('watchDataMetrics', () => {
    it('retrieves data metrics', async () => {
      await anAccount().store(db);
      await aCategory().store(db);
      await aCashflow().store(db);
      await aStoreItem().store(db);
      await aTransaction().store(db);
      await db.execute('INSERT INTO ps_crud (id) VALUES (1);');
      const repo = SettingRepositoryInPowersync(db);

      let result: AsyncResult<DataMetrics> | undefined;
      const unsubscribe = repo.watchDataMetrics((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue({
        accounts: 1,
        cashflows: 1,
        categories: 1,
        storeItems: 1,
        transactions: 1,
        outstandingTransactions: 1,
      });

      unsubscribe();
    });

    it('returns not found when no data metrics exist', async () => {
      const repo = SettingRepositoryInPowersync(db);

      let result: AsyncResult<DataMetrics> | undefined;
      const unsubscribe = repo.watchDataMetrics((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue({
        accounts: 0,
        cashflows: 0,
        categories: 0,
        storeItems: 0,
        transactions: 0,
        outstandingTransactions: 0,
      });

      unsubscribe();
    });

    it('handles database errors', async () => {
      const repo = SettingRepositoryInPowersync(db);

      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<DataMetrics> | undefined;
      const unsubscribe = repo.watchDataMetrics((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });
});
