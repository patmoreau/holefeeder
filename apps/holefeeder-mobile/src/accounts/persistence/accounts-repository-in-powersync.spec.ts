import { waitFor } from '@testing-library/react-native';
import { anAccount } from '@/accounts/core/__tests__/account-for-test';
import { Account } from '@/accounts/core/account';
import { AccountsRepositoryErrors } from '@/accounts/core/accounts-repository';
import { anUpdateAccountCommand } from '@/accounts/core/update/__tests__/update-account-command-for-test';
import { type AsyncResult } from '@/shared/core/result';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { AccountsRepositoryInPowersync } from './accounts-repository-in-powersync';

describe('AccountsRepositoryInPowersync', () => {
  let db: DatabaseForTest;

  beforeEach(async () => {
    db = await setupDatabaseForTest();
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  describe('watch', () => {
    it('retrieves a stored account', async () => {
      const account = await anAccount().store(db);
      const repo = AccountsRepositoryInPowersync(db);

      let result: AsyncResult<Account[]> | undefined;
      const unsubscribe = repo.watch((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([
        {
          id: account.id,
          type: account.type,
          name: account.name,
          openBalance: account.openBalance,
          openDate: account.openDate,
          description: account.description,
          favorite: account.favorite,
          inactive: account.inactive,
        },
      ]);

      unsubscribe();
    });

    it('returns not found when no accounts exist', async () => {
      const repo = AccountsRepositoryInPowersync(db);

      let result: AsyncResult<Account[]> | undefined;
      const unsubscribe = repo.watch((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      const repo = AccountsRepositoryInPowersync(db);

      await db.close();

      let result: AsyncResult<any> | undefined;
      const unsubscribe = repo.watch((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('update', () => {
    it('updates an existing account', async () => {
      const account = await anAccount().store(db);
      const repo = AccountsRepositoryInPowersync(db);
      const command = anUpdateAccountCommand({ id: account.id, name: 'New Name', type: account.type });

      const result = await repo.update(command);

      expect(result).toBeSuccessWithValue(account.id);
    });

    it('returns failure when account does not exist', async () => {
      const repo = AccountsRepositoryInPowersync(db);
      const command = anUpdateAccountCommand();

      const result = await repo.update(command);

      expect(result).toBeFailureWithErrors([AccountsRepositoryErrors.accountNotFound]);
    });

    it('returns failure on database error', async () => {
      const repo = AccountsRepositoryInPowersync(db);
      await db.close();

      const command = anUpdateAccountCommand();
      const result = await repo.update(command);

      expect(result.isFailure).toBe(true);
    });
  });
});
