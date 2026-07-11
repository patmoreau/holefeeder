import { type AsyncResult, DateIntervalTypes, DateOnly, Money, withDate } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { startOfMonth } from 'date-fns';
import { aCategory, CategoryForTest } from '@/flows/core/categories/__tests__/category-for-test';
import { aTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { TagList } from '@/flows/core/flows/tag-list';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { System } from '@/shared/core/system';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { InsightsRepository } from '@/statistics/core/insights-repository';
import { CategorySpending } from '../core/category-spending';
import { TagSpending } from '../core/tag-spending';
import { InsightsRepositoryInPowersync } from './insights-repository-in-powersync';

describe('InsightsRepositoryInPowersync', () => {
  const month1 = withDate(startOfMonth(new Date(2026, 0, 1))).toDateOnly();
  const month2 = withDate(startOfMonth(new Date(2026, 1, 1))).toDateOnly();
  const month3 = withDate(startOfMonth(new Date(2026, 2, 1))).toDateOnly();
  const month4 = withDate(startOfMonth(new Date(2026, 3, 1))).toDateOnly();
  const month5 = withDate(startOfMonth(new Date(2026, 4, 1))).toDateOnly();
  const periodStart = withDate(new Date(2026, 4, 1)).toDateOnly();
  const effectiveDate = withDate(periodStart).addDays(5).toDateOnly();
  const settings = aSettings({
    effectiveDate: month1,
    intervalType: DateIntervalTypes.monthly,
    frequency: 1,
  });
  let db: DatabaseForTest;
  let repo: InsightsRepository;
  let foodCategory: CategoryForTest;
  let homeCategory: CategoryForTest;
  // let rareCategory: CategoryForTest;

  beforeEach(async () => {
    db = await setupDatabaseForTest();
    repo = InsightsRepositoryInPowersync(db);

    foodCategory = await aCategory({ name: 'Food', type: 'expense', color: '#ff0000', budgetAmount: Money.valid(500) }).store(db);
    homeCategory = await aCategory({ name: 'Home', type: 'expense', color: '#00ff00' }).store(db);

    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(100),
      date: withDate(month1).addDays(1).toDateOnly(),
      tags: TagList.valid(['groceries', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(101),
      date: withDate(month1).addDays(11).toDateOnly(),
      tags: TagList.valid(['restaurant', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(102),
      date: withDate(month2).addDays(2).toDateOnly(),
      tags: TagList.valid(['groceries', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(103),
      date: withDate(month2).addDays(12).toDateOnly(),
      tags: TagList.valid(['restaurant', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(104),
      date: withDate(month3).addDays(3).toDateOnly(),
      tags: TagList.valid(['groceries', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(105),
      date: withDate(month3).addDays(13).toDateOnly(),
      tags: TagList.valid(['restaurant', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(106),
      date: withDate(month4).addDays(4).toDateOnly(),
      tags: TagList.valid(['groceries', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(107),
      date: withDate(month4).addDays(14).toDateOnly(),
      tags: TagList.valid(['restaurant', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(108),
      date: withDate(month5).addDays(5).toDateOnly(),
      tags: TagList.valid(['groceries', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: foodCategory.id,
      amount: Money.valid(109),
      date: withDate(month5).addDays(15).toDateOnly(),
      tags: TagList.valid(['restaurant', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: homeCategory.id,
      amount: Money.valid(500),
      date: withDate(month1).addDays(1).toDateOnly(),
      tags: TagList.valid(['utilities', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: homeCategory.id,
      amount: Money.valid(501),
      date: withDate(month2).addDays(1).toDateOnly(),
      tags: TagList.valid(['utilities', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: homeCategory.id,
      amount: Money.valid(502),
      date: withDate(month3).addDays(10).toDateOnly(),
      tags: TagList.valid(['utilities', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: homeCategory.id,
      amount: Money.valid(503),
      date: withDate(month4).addDays(18).toDateOnly(),
      tags: TagList.valid(['utilities', 'travel']),
    }).store(db);
    await aTransaction({
      categoryId: homeCategory.id,
      amount: Money.valid(504),
      date: withDate(month5).addDays(25).toDateOnly(),
      tags: TagList.valid(['utilities', 'travel']),
    }).store(db);
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  describe('watch category spending', () => {
    let result: AsyncResult<CategorySpending[]> | undefined;

    const watchCategorySpending = (effectiveDate: DateOnly) => {
      return repo.watchCategorySpending(
        (data) => {
          result = data;
        },
        effectiveDate,
        settings
      );
    };

    it('returns category spending when there is no previous period', async () => {
      const unsubscribe = watchCategorySpending(settings.effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ categoryName: 'Home', spentAmount: Money.valid(500), avgAmount: Money.ZERO }),
        expect.objectContaining({ categoryName: 'Food', spentAmount: Money.valid(201), avgAmount: Money.ZERO }),
      ]);

      unsubscribe();
    });

    it('returns category spending with average of previous periods', async () => {
      const unsubscribe = watchCategorySpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ categoryName: 'Home', spentAmount: Money.valid(504), avgAmount: Money.valid(501.5) }),
        expect.objectContaining({ categoryName: 'Food', spentAmount: Money.valid(217), avgAmount: Money.valid(207) }),
      ]);

      unsubscribe();
    });

    it('includes ZERO spent amount', async () => {
      const rareCategory = await aCategory({ name: 'Rare', type: 'expense' }).store(db);

      await aTransaction({
        categoryId: rareCategory.id,
        amount: Money.valid(1000),
        date: month3,
        tags: TagList.valid(['taxes']),
      }).store(db);

      const unsubscribe = watchCategorySpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ categoryName: 'Home' }),
        expect.objectContaining({ categoryName: 'Rare', spentAmount: Money.ZERO, avgAmount: Money.valid(500) }),
        expect.objectContaining({ categoryName: 'Food' }),
      ]);

      unsubscribe();
    });

    it('excludes gain categories', async () => {
      const gainCategory = await aCategory({ name: 'Salary', type: 'gain' }).store(db);

      await aTransaction({ categoryId: gainCategory.id, amount: Money.valid(200), date: periodStart }).store(db);

      const unsubscribe = watchCategorySpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ categoryName: 'Home' }),
        expect.objectContaining({ categoryName: 'Food' }),
      ]);

      unsubscribe();
    });

    it('excludes system categories', async () => {
      const systemCategory = await aCategory({ name: 'Transfer', type: 'expense', system: System.valid(true) }).store(db);

      await aTransaction({ categoryId: systemCategory.id, amount: Money.valid(300), date: periodStart }).store(db);

      const unsubscribe = watchCategorySpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ categoryName: 'Home' }),
        expect.objectContaining({ categoryName: 'Food' }),
      ]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      await db.close();

      const unsubscribe = watchCategorySpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });

  describe('watch tag spending', () => {
    let result: AsyncResult<TagSpending[]> | undefined;

    const watchTagSpending = (effectiveDate: DateOnly) => {
      return repo.watchTagSpending(
        (data) => {
          result = data;
        },
        effectiveDate,
        settings
      );
    };

    it('returns tag spending summed across all categories when there is no previous periods', async () => {
      const unsubscribe = watchTagSpending(settings.effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ tag: 'travel', spentAmount: Money.valid(701), avgAmount: Money.ZERO }),
        expect.objectContaining({ tag: 'utilities', spentAmount: Money.valid(500), avgAmount: Money.ZERO }),
        expect.objectContaining({ tag: 'restaurant', spentAmount: Money.valid(101), avgAmount: Money.ZERO }),
        expect.objectContaining({ tag: 'groceries', spentAmount: Money.valid(100), avgAmount: Money.ZERO }),
      ]);

      unsubscribe();
    });

    it('returns tag spending summed across all categories with average of the previous period', async () => {
      const unsubscribe = watchTagSpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ tag: 'travel', spentAmount: Money.valid(721), avgAmount: Money.valid(708.5) }),
        expect.objectContaining({ tag: 'utilities', spentAmount: Money.valid(504), avgAmount: Money.valid(501.5) }),
        expect.objectContaining({ tag: 'restaurant', spentAmount: Money.valid(109), avgAmount: Money.valid(104) }),
        expect.objectContaining({ tag: 'groceries', spentAmount: Money.valid(108), avgAmount: Money.valid(103) }),
      ]);

      unsubscribe();
    });

    it('includes ZERO spent amount', async () => {
      const rareCategory = await aCategory({ name: 'Rare', type: 'expense' }).store(db);

      await aTransaction({
        categoryId: rareCategory.id,
        amount: Money.valid(1000),
        date: month3,
        tags: TagList.valid(['taxes']),
      }).store(db);

      const unsubscribe = watchTagSpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ tag: 'travel' }),
        expect.objectContaining({ tag: 'utilities' }),
        expect.objectContaining({ tag: 'taxes', spentAmount: Money.ZERO, avgAmount: Money.valid(500) }),
        expect.objectContaining({ tag: 'restaurant' }),
        expect.objectContaining({ tag: 'groceries' }),
      ]);

      unsubscribe();
    });

    it('excludes transactions from gain categories', async () => {
      const gainCategory = await aCategory({ name: 'Salary', type: 'gain' }).store(db);

      await aTransaction({
        categoryId: gainCategory.id,
        amount: Money.valid(1),
        date: periodStart,
        tags: TagList.valid(['other']),
      }).store(db);

      const unsubscribe = watchTagSpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ tag: 'travel' }),
        expect.objectContaining({ tag: 'utilities' }),
        expect.objectContaining({ tag: 'restaurant' }),
        expect.objectContaining({ tag: 'groceries' }),
      ]);

      unsubscribe();
    });

    it('excludes transactions from system categories', async () => {
      const systemCategory = await aCategory({ name: 'Transfer', type: 'expense', system: true as System }).store(db);

      await aTransaction({
        categoryId: systemCategory.id,
        amount: Money.valid(300),
        date: periodStart,
        tags: TagList.valid(['food']),
      }).store(db);

      const unsubscribe = watchTagSpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue([
        expect.objectContaining({ tag: 'travel' }),
        expect.objectContaining({ tag: 'utilities' }),
        expect.objectContaining({ tag: 'restaurant' }),
        expect.objectContaining({ tag: 'groceries' }),
      ]);

      unsubscribe();
    });

    it('handles database errors', async () => {
      await db.close();

      const unsubscribe = watchTagSpending(effectiveDate);

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeFailureWithErrors(['The database connection is not open']);

      unsubscribe();
    });
  });
});
