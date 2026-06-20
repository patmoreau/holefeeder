import { type AsyncResult, DateInterval, Money, today } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { Settings } from '@/settings/core/settings';
import { watchQuery } from '@/shared/persistence/watch-query';
import { CategorySpending } from '../core/category-spending';
import { InsightsRepository } from '../core/insights-repository';

type CategorySpendingRow = {
  categoryId: string;
  categoryName: string;
  color: string;
  budgetAmount: number;
  spentAmount: number;
};

export const InsightsRepositoryInPowersync = (db: AbstractPowerSyncDatabase): InsightsRepository => {
  const watchCategorySpending = (onDataChange: (result: AsyncResult<CategorySpending[]>) => void, settings: Settings) => {
    const intervalResult = DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency);
    if (!intervalResult.isSuccess) {
      onDataChange(intervalResult);
      return () => {};
    }

    const { start, end } = intervalResult.value;

    return watchQuery<CategorySpendingRow, CategorySpending>(
      db,
      `
        SELECT
          c.id          AS categoryId,
          c.name        AS categoryName,
          c.color,
          c.budget_amount AS budgetAmount,
          COALESCE(SUM(t.amount), 0) AS spentAmount
        FROM categories c
        LEFT JOIN transactions t
          ON t.category_id = c.id AND t.date >= ? AND t.date <= ?
        WHERE c.system = 0 AND lower(c.type) = 'expense'
        GROUP BY c.id, c.name, c.color, c.budget_amount
        ORDER BY spentAmount DESC
      `,
      [start, end],
      (row) => ({
        categoryId: row.categoryId,
        categoryName: row.categoryName,
        color: row.color,
        budgetAmount: Money.fromCents(row.budgetAmount),
        spentAmount: Money.fromCents(row.spentAmount),
      }),
      onDataChange,
      'watchCategorySpending'
    );
  };

  return { watchCategorySpending: watchCategorySpending };
};
