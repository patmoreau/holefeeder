import { type AsyncResult, DateIntervalTypes, Money, today, withDate } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { startOfMonth } from 'date-fns';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { System } from '@/shared/core/system';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { CategorySpending } from '../core/category-spending';
import { TagSpending } from '../core/tag-spending';
import { InsightsRepositoryInPowersync } from './insights-repository-in-powersync';

describe('InsightsRepositoryInPowersync', () => {
  let db: DatabaseForTest;
  const periodStart = withDate(startOfMonth(today())).toDateOnly();
  const periodEnd = withDate(periodStart).addMonths(1).addDays(-1).toDateOnly();
  const outsidePeriod = withDate(periodStart).addMonths(-1).toDateOnly();
  const settings = aSettings({
    effectiveDate: periodStart,
    intervalType: DateIntervalTypes.monthly,
    frequency: 1,
  });

  beforeEach(async () => {
    db = await setupDatabaseForTest();
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  describe('watchCategorySpending', () => {
    it('returns category spending for the current period', async () => {
      const foodCategory = await aCategory({ name: 'Food', type: 'expense', color: '#ff0000', budgetAmount: Money.valid(500) }).store(db);
      const transportCategory = await aCategory({ name: 'Transport', type: 'expense', color: '#00ff00' }).store(db);

      await aTransaction({ categoryId: foodCategory.id, amount: Money.valid(100), date: periodStart }).store(db);
      await aTransaction({ categoryId: foodCategory.id, amount: Money.valid(50), date: periodEnd }).store(db);
      await aTransaction({ categoryId: transportCategory.id, amount: Money.valid(30), date: periodStart }).store(db);
      await aTransaction({ categoryId: foodCategory.id, amount: Money.valid(999), date: outsidePeriod }).store(db);

      const repo = InsightsRepositoryInPowersync(db);
      let result: AsyncResult<CategorySpending[]> | undefined;
      const unsubscribe = repo.watchCategorySpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ categoryName: 'Food', spentAmount: Money.valid(150) }),
        expect.objectContaining({ categoryName: 'Transport', spentAmount: Money.valid(30) }),
      ]);

      unsubscribe();
    });

    it('excludes gain and system categories', async () => {
      const expenseCategory = await aCategory({ name: 'Food', type: 'expense' }).store(db);
      const gainCategory = await aCategory({ name: 'Salary', type: 'gain' }).store(db);
      const systemCategory = await aCategory({ name: 'Transfer', type: 'expense', system: true as System }).store(db);

      await aTransaction({ categoryId: expenseCategory.id, amount: Money.valid(100), date: periodStart }).store(db);
      await aTransaction({ categoryId: gainCategory.id, amount: Money.valid(200), date: periodStart }).store(db);
      await aTransaction({ categoryId: systemCategory.id, amount: Money.valid(300), date: periodStart }).store(db);

      const repo = InsightsRepositoryInPowersync(db);
      let result: AsyncResult<CategorySpending[]> | undefined;
      const unsubscribe = repo.watchCategorySpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([expect.objectContaining({ categoryName: 'Food' })]);

      unsubscribe();
    });

    it('includes expense categories with zero spend in period', async () => {
      await aCategory({ name: 'Food', type: 'expense' }).store(db);

      const repo = InsightsRepositoryInPowersync(db);
      let result: AsyncResult<CategorySpending[]> | undefined;
      const unsubscribe = repo.watchCategorySpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([expect.objectContaining({ categoryName: 'Food', spentAmount: Money.ZERO })]);

      unsubscribe();
    });

    it('returns empty list when no categories exist', async () => {
      const repo = InsightsRepositoryInPowersync(db);
      let result: AsyncResult<CategorySpending[]> | undefined;
      const unsubscribe = repo.watchCategorySpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      const repo = InsightsRepositoryInPowersync(db);
      await db.close();

      let result: AsyncResult<CategorySpending[]> | undefined;
      const unsubscribe = repo.watchCategorySpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('watchTagSpending', () => {
    it('returns tag spending summed across all categories for the current period', async () => {
      const foodCategory = await aCategory({ name: 'Food', type: 'expense' }).store(db);
      const transportCategory = await aCategory({ name: 'Transport', type: 'expense' }).store(db);

      await aTransaction({ categoryId: foodCategory.id, amount: Money.valid(100), date: periodStart, tags: ['groceries', 'weekly'] }).store(db);
      await aTransaction({ categoryId: transportCategory.id, amount: Money.valid(30), date: periodStart, tags: ['groceries'] }).store(db);
      await aTransaction({ categoryId: foodCategory.id, amount: Money.valid(999), date: outsidePeriod, tags: ['groceries'] }).store(db);

      const repo = InsightsRepositoryInPowersync(db);
      let result: AsyncResult<TagSpending[]> | undefined;
      const unsubscribe = repo.watchTagSpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ tag: 'groceries', spentAmount: Money.valid(130) }),
        expect.objectContaining({ tag: 'weekly', spentAmount: Money.valid(100) }),
      ]);

      unsubscribe();
    });

    it('excludes transactions from gain and system categories', async () => {
      const expenseCategory = await aCategory({ name: 'Food', type: 'expense' }).store(db);
      const gainCategory = await aCategory({ name: 'Salary', type: 'gain' }).store(db);
      const systemCategory = await aCategory({ name: 'Transfer', type: 'expense', system: true as System }).store(db);

      await aTransaction({ categoryId: expenseCategory.id, amount: Money.valid(100), date: periodStart, tags: ['food'] }).store(db);
      await aTransaction({ categoryId: gainCategory.id, amount: Money.valid(200), date: periodStart, tags: ['food'] }).store(db);
      await aTransaction({ categoryId: systemCategory.id, amount: Money.valid(300), date: periodStart, tags: ['food'] }).store(db);

      const repo = InsightsRepositoryInPowersync(db);
      let result: AsyncResult<TagSpending[]> | undefined;
      const unsubscribe = repo.watchTagSpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([expect.objectContaining({ tag: 'food', spentAmount: Money.valid(100) })]);

      unsubscribe();
    });

    it('returns empty list when no tagged transactions exist in period', async () => {
      const foodCategory = await aCategory({ name: 'Food', type: 'expense' }).store(db);
      await aTransaction({ categoryId: foodCategory.id, amount: Money.valid(50), date: periodStart, tags: [] }).store(db);

      const repo = InsightsRepositoryInPowersync(db);
      let result: AsyncResult<TagSpending[]> | undefined;
      const unsubscribe = repo.watchTagSpending((data) => {
        result = data;
      }, settings);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([]);

      unsubscribe();
    });
  });
});
