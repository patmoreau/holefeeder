import { type AsyncResult, Inactive, Money } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { aCashflow } from '@/flows/core/flows/__tests__/cashflow-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { TagList } from '@/flows/core/flows/tag-list';
import { TagInfo } from '@/flows/core/tags/tag-info';
import { TagsRepository } from '@/flows/core/tags/tags-repository';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { TagsRepositoryInPowersync } from './tags-repository-in-powersync';

describe('TagsRepositoryInPowersync', () => {
  let db: DatabaseForTest;
  let repo: TagsRepository;
  let result: AsyncResult<TagInfo[]> | undefined;

  const watchTags = () =>
    repo.watch((data) => {
      result = data;
    });

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    repo = TagsRepositoryInPowersync(db);
    result = undefined;
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  it('lists distinct tags with transaction and active cashflow counts', async () => {
    await aTransaction({ amount: Money.valid(10), tags: TagList.valid(['groceries', 'travel']) }).store(db);
    await aTransaction({ amount: Money.valid(20), tags: TagList.valid(['groceries']) }).store(db);
    await aCashflow({ tags: TagList.valid(['groceries', 'rent']), inactive: false as Inactive }).store(db);
    await aCashflow({ tags: TagList.valid(['rent']), inactive: false as Inactive }).store(db);

    const unsubscribe = watchTags();

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue([
      { tag: 'groceries', transactionCount: 2, activeCashflowCount: 1 },
      { tag: 'rent', transactionCount: 0, activeCashflowCount: 2 },
      { tag: 'travel', transactionCount: 1, activeCashflowCount: 0 },
    ]);

    unsubscribe();
  });

  it('counts only active cashflows', async () => {
    await aCashflow({ tags: TagList.valid(['rent']), inactive: false as Inactive }).store(db);
    await aCashflow({ tags: TagList.valid(['rent']), inactive: true as Inactive }).store(db);

    const unsubscribe = watchTags();

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue([{ tag: 'rent', transactionCount: 0, activeCashflowCount: 1 }]);

    unsubscribe();
  });

  it('ignores transactions and cashflows without tags', async () => {
    await aTransaction({ tags: TagList.valid([]) }).store(db);
    await aCashflow({ tags: TagList.valid([]), inactive: false as Inactive }).store(db);
    await aTransaction({ tags: TagList.valid(['solo']) }).store(db);

    const unsubscribe = watchTags();

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeSuccessWithValue([{ tag: 'solo', transactionCount: 1, activeCashflowCount: 0 }]);

    unsubscribe();
  });

  it('handles database errors', async () => {
    await db.close();

    const unsubscribe = watchTags();

    await waitFor(() => expect(result).toBeDefined());

    expect(result).toBeFailureWithErrors(['The database connection is not open']);

    unsubscribe();
  });

  describe('rename', () => {
    const tagsOf = async (table: 'transactions' | 'cashflows', id: string): Promise<string> => {
      const rows = await db.getAll<{ tags: string }>(`SELECT tags FROM ${table} WHERE id = ?`, [id]);
      return rows[0].tags;
    };

    it('renames the tag across transactions and cashflows that have it exactly', async () => {
      const tx1 = await aTransaction({ tags: TagList.valid(['groceries', 'travel']) }).store(db);
      const tx2 = await aTransaction({ tags: TagList.valid(['groceries']) }).store(db);
      const cf1 = await aCashflow({ tags: TagList.valid(['groceries', 'rent']), inactive: false as Inactive }).store(db);

      const renameResult = await repo.rename('groceries', 'food');

      expect(renameResult).toBeSuccessWithValue(undefined);
      expect(await tagsOf('transactions', tx1.id as string)).toBe('food,travel');
      expect(await tagsOf('transactions', tx2.id as string)).toBe('food');
      expect(await tagsOf('cashflows', cf1.id as string)).toBe('food,rent');
    });

    it('renames inactive cashflows as well', async () => {
      const cf = await aCashflow({ tags: TagList.valid(['groceries']), inactive: true as Inactive }).store(db);

      await repo.rename('groceries', 'food');

      expect(await tagsOf('cashflows', cf.id as string)).toBe('food');
    });

    it('does not rename tags that only contain the old tag as a substring', async () => {
      const tx = await aTransaction({ tags: TagList.valid(['travelling', 'travel']) }).store(db);

      await repo.rename('travel', 'trip');

      expect(await tagsOf('transactions', tx.id as string)).toBe('travelling,trip');
    });

    it('preserves the position of other tags in the list', async () => {
      const tx = await aTransaction({ tags: TagList.valid(['first', 'groceries', 'last']) }).store(db);

      await repo.rename('groceries', 'food');

      expect(await tagsOf('transactions', tx.id as string)).toBe('first,food,last');
    });

    it('merges without duplicating when the new tag already exists on the same row', async () => {
      const tx = await aTransaction({ tags: TagList.valid(['food', 'groceries']) }).store(db);

      await repo.rename('groceries', 'food');

      expect(await tagsOf('transactions', tx.id as string)).toBe('food');
    });

    it('handles database errors', async () => {
      await db.close();

      const renameResult = await repo.rename('groceries', 'food');

      expect(renameResult).toBeFailureWithErrors(['The database connection is not open']);
    });
  });
});
