import { type AsyncResult, DateInterval, Money, today } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { Settings } from '@/settings/core/settings';
import { watchQuery } from '@/shared/persistence/watch-query';
import { CategorySpending } from '../core/category-spending';
import { InsightsRepository } from '../core/insights-repository';
import { TagSpending } from '../core/tag-spending';

type CategorySpendingRow = {
  categoryId: string;
  categoryName: string;
  color: string;
  budgetAmount: number;
  spentAmount: number;
};

type TagSpendingRow = {
  tag: string;
  spentAmount: number;
};

const computeInterval = (settings: Settings) =>
  DateInterval.createFrom(today(), 0, settings.effectiveDate, settings.intervalType, settings.frequency);

export const InsightsRepositoryInPowersync = (db: AbstractPowerSyncDatabase): InsightsRepository => {
  const watchCategorySpending = (onDataChange: (result: AsyncResult<CategorySpending[]>) => void, settings: Settings) => {
    const intervalResult = computeInterval(settings);
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

  const watchTagSpending = (onDataChange: (result: AsyncResult<TagSpending[]>) => void, settings: Settings) => {
    const intervalResult = computeInterval(settings);
    if (!intervalResult.isSuccess) {
      onDataChange(intervalResult);
      return () => {};
    }

    const { start, end } = intervalResult.value;

    return watchQuery<TagSpendingRow, TagSpending>(
      db,
      `
        WITH RECURSIVE split(tag, remainder, amount) AS (
          SELECT
            Ltrim(Substr(t.tags || ',', 1, Instr(t.tags || ',', ',') - 1)) AS tag,
            Substr(t.tags || ',', Instr(t.tags || ',', ',') + 1)           AS remainder,
            t.amount
          FROM transactions t
          JOIN categories c ON c.id = t.category_id
          WHERE t.tags IS NOT NULL AND t.tags <> ''
            AND t.date >= ? AND t.date <= ?
            AND lower(c.type) = 'expense' AND c.system = 0
          UNION ALL
          SELECT
            Ltrim(Substr(remainder, 1, Instr(remainder, ',') - 1)) AS tag,
            Substr(remainder, Instr(remainder, ',') + 1)           AS remainder,
            amount
          FROM split
          WHERE remainder <> ''
        )
        SELECT tag, SUM(amount) AS spentAmount
        FROM split
        WHERE tag <> ''
        GROUP BY tag
        ORDER BY spentAmount DESC
      `,
      [start, end],
      (row) => ({ tag: row.tag, spentAmount: Money.fromCents(row.spentAmount) }),
      onDataChange,
      'watchTagSpending'
    );
  };

  return { watchCategorySpending: watchCategorySpending, watchTagSpending: watchTagSpending };
};
