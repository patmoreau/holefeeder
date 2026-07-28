import { type AsyncResult, DateInterval, DateIntervalType, DateOnly, Id, Money } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { Settings } from '@/shared/core/settings';
import { watchQuery } from '@/shared/persistence/watch-query';
import { CategorySpending } from '../core/category-spending';
import { CategoryTagSpending } from '../core/category-tag-spending';
import { InsightsRepository } from '../core/insights-repository';
import { TagSpending } from '../core/tag-spending';

type CategorySpendingRow = {
  categoryId: string;
  categoryName: string;
  color: string;
  budgetAmount: number;
  spentAmount: number;
  avgAmount: number;
};

type TagSpendingRow = {
  tag: string;
  spentAmount: number;
  avgAmount: number;
};

type CategoryTagSpendingRow = {
  categoryId: string;
  tag: string;
  spentAmount: number;
};

type Period = { start: DateOnly; end: DateOnly };
type BuiltQuery = { sql: string; params: (string | number)[] };

const computeInterval = (effectiveDate: DateOnly, settings: Settings) =>
  DateInterval.createFrom(effectiveDate, 0, settings.effectiveDate, settings.intervalType, settings.frequency);

const previousPeriods = (settings: Settings, currentStart: DateOnly): Period[] =>
  DateIntervalType.previousPeriods(settings.effectiveDate, settings.intervalType, settings.frequency, currentStart);

const buildCategorySpendingQuery = (periods: Period[], currentStart: DateOnly, currentEnd: DateOnly): BuiltQuery => {
  if (periods.length === 0) {
    return {
      sql: `
        SELECT
          c.id                       AS categoryId,
          c.name                     AS categoryName,
          c.color                    AS color,
          c.budget_amount            AS budgetAmount,
          COALESCE(SUM(t.amount), 0) AS spentAmount,
          0 AS avgAmount
        FROM categories c
        INNER JOIN transactions t ON t.category_id = c.id AND t.date >= ? AND t.date <= ?
        WHERE c.system = 0 AND lower(c.type) = 'expense'
        GROUP BY c.id, c.name, c.color, c.budget_amount
        ORDER BY avgAmount DESC, spentAmount DESC, c.name ASC
      `,
      params: [currentStart, currentEnd],
    };
  }

  const valuesClause = periods.map(() => '(?, ?, ?)').join(', ');
  return {
    sql: `
      WITH period_defs(idx, period_start, period_end) AS (
        VALUES ${valuesClause}
      ),
      period_sums AS (
        SELECT t.category_id, pd.idx, SUM(t.amount) AS periodTotal
        FROM transactions t
        JOIN period_defs pd ON t.date >= pd.period_start AND t.date < pd.period_end
        GROUP BY t.category_id, pd.idx
      ),
      avg_periods AS (
        SELECT
          category_id,
          SUM(periodTotal) * 1.0 / ((SELECT MAX(idx) FROM period_defs) - MIN(idx) + 1) AS avgAmount
        FROM period_sums
        GROUP BY category_id
      ),
      current_period AS (
        SELECT category_id, SUM(amount) AS spentAmount
        FROM transactions
        WHERE date >= ? AND date <= ?
        GROUP BY category_id
      )
      SELECT
        c.id            AS categoryId,
        c.name          AS categoryName,
        c.color,
        c.budget_amount AS budgetAmount,
        COALESCE(cp.spentAmount, 0)      AS spentAmount,
        ROUND(COALESCE(ap.avgAmount, 0)) AS avgAmount
      FROM categories c
      LEFT JOIN current_period cp ON cp.category_id = c.id
      LEFT JOIN avg_periods    ap ON ap.category_id = c.id
      WHERE c.system = 0 AND lower(c.type) = 'expense'
      ORDER BY avgAmount DESC, spentAmount DESC, c.name ASC
    `,
    params: [...periods.flatMap((p, i) => [i, p.start, p.end]), currentStart, currentEnd],
  };
};

const buildTagSpendingQuery = (periods: Period[], currentStart: DateOnly, currentEnd: DateOnly): BuiltQuery => {
  if (periods.length === 0) {
    return {
      sql: `
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
        SELECT tag, SUM(amount) AS spentAmount, 0 AS avgAmount
        FROM split
        WHERE tag <> ''
        GROUP BY tag
        ORDER BY spentAmount DESC
      `,
      params: [currentStart, currentEnd],
    };
  }

  const valuesClause = periods.map(() => '(?, ?, ?)').join(', ');
  return {
    sql: `
      WITH RECURSIVE
      period_defs(idx, period_start, period_end) AS (
        VALUES ${valuesClause}
      ),
      period_split(idx, tag, remainder, amount) AS (
        SELECT
          pd.idx,
          Ltrim(Substr(t.tags || ',', 1, Instr(t.tags || ',', ',') - 1)) AS tag,
          Substr(t.tags || ',', Instr(t.tags || ',', ',') + 1)           AS remainder,
          t.amount
        FROM transactions t
        JOIN categories c ON c.id = t.category_id
        JOIN period_defs pd ON t.date >= pd.period_start AND t.date < pd.period_end
        WHERE t.tags IS NOT NULL AND t.tags <> ''
          AND lower(c.type) = 'expense' AND c.system = 0
        UNION ALL
        SELECT
          idx,
          Ltrim(Substr(remainder, 1, Instr(remainder, ',') - 1)) AS tag,
          Substr(remainder, Instr(remainder, ',') + 1)           AS remainder,
          amount
        FROM period_split
        WHERE remainder <> ''
      ),
      period_sums AS (
        SELECT idx, tag, SUM(amount) AS periodTotal
        FROM period_split
        WHERE tag <> ''
        GROUP BY idx, tag
      ),
      avg_periods AS (
        SELECT
          tag,
          SUM(periodTotal) * 1.0 / ((SELECT MAX(idx) FROM period_defs) - MIN(idx) + 1) AS avgAmount
        FROM period_sums
        GROUP BY tag
      ),
      current_split(tag, remainder, amount) AS (
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
        FROM current_split
        WHERE remainder <> ''
      ),
      current_tags AS (
        SELECT tag, SUM(amount) AS spentAmount
        FROM current_split
        WHERE tag <> ''
        GROUP BY tag
      ),
      all_tags AS (
        SELECT tag FROM current_tags
        UNION
        SELECT tag FROM avg_periods
      )
      SELECT
        at.tag                           AS tag,
        COALESCE(ct.spentAmount, 0)      AS spentAmount,
        ROUND(COALESCE(ap.avgAmount, 0)) AS avgAmount
      FROM all_tags at
      LEFT JOIN current_tags ct ON ct.tag = at.tag
      LEFT JOIN avg_periods  ap ON ap.tag = at.tag
      ORDER BY avgAmount DESC, spentAmount DESC, at.tag ASC
    `,
    params: [...periods.flatMap((p, i) => [i, p.start, p.end]), currentStart, currentEnd],
  };
};

const buildCategoryTagSpendingQuery = (currentStart: DateOnly, currentEnd: DateOnly): BuiltQuery => ({
  sql: `
    WITH RECURSIVE split(category_id, tag, remainder, amount) AS (
      SELECT
        t.category_id,
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
        category_id,
        Ltrim(Substr(remainder, 1, Instr(remainder, ',') - 1)) AS tag,
        Substr(remainder, Instr(remainder, ',') + 1)           AS remainder,
        amount
      FROM split
      WHERE remainder <> ''
    )
    SELECT
      s.category_id        AS categoryId,
      s.tag                AS tag,
      SUM(s.amount)        AS spentAmount
    FROM split s
    JOIN categories c ON c.id = s.category_id
    WHERE s.tag <> ''
    GROUP BY s.category_id, s.tag
    ORDER BY c.name ASC, spentAmount DESC, s.tag ASC
  `,
  params: [currentStart, currentEnd],
});

export const InsightsRepositoryInPowersync = (db: AbstractPowerSyncDatabase): InsightsRepository => {
  const watchCategorySpending = (
    onDataChange: (result: AsyncResult<CategorySpending[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => {
    const intervalResult = computeInterval(effectiveDate, settings);
    if (!intervalResult.isSuccess) {
      onDataChange(intervalResult);
      return () => {};
    }

    const { start, end } = intervalResult.value;
    const { sql, params } = buildCategorySpendingQuery(previousPeriods(settings, start), start, end);

    return watchQuery<CategorySpendingRow, CategorySpending>(
      db,
      sql,
      params,
      (row) => ({
        categoryId: Id.valid(row.categoryId),
        categoryName: row.categoryName,
        color: row.color,
        budgetAmount: Money.fromCents(row.budgetAmount),
        spentAmount: Money.fromCents(row.spentAmount),
        avgAmount: Money.fromCents(row.avgAmount),
      }),
      onDataChange,
      'watchCategorySpending'
    );
  };

  const watchTagSpending = (onDataChange: (result: AsyncResult<TagSpending[]>) => void, effectiveDate: DateOnly, settings: Settings) => {
    const intervalResult = computeInterval(effectiveDate, settings);
    if (!intervalResult.isSuccess) {
      onDataChange(intervalResult);
      return () => {};
    }

    const { start, end } = intervalResult.value;
    const { sql, params } = buildTagSpendingQuery(previousPeriods(settings, start), start, end);

    return watchQuery<TagSpendingRow, TagSpending>(
      db,
      sql,
      params,
      (row) => ({ tag: row.tag, spentAmount: Money.fromCents(row.spentAmount), avgAmount: Money.fromCents(row.avgAmount) }),
      onDataChange,
      'watchTagSpending'
    );
  };

  const watchCategoryTagSpending = (
    onDataChange: (result: AsyncResult<CategoryTagSpending[]>) => void,
    effectiveDate: DateOnly,
    settings: Settings
  ) => {
    const intervalResult = computeInterval(effectiveDate, settings);
    if (!intervalResult.isSuccess) {
      onDataChange(intervalResult);
      return () => {};
    }

    const { start, end } = intervalResult.value;
    const { sql, params } = buildCategoryTagSpendingQuery(start, end);

    return watchQuery<CategoryTagSpendingRow, CategoryTagSpending>(
      db,
      sql,
      params,
      (row) => ({ categoryId: Id.valid(row.categoryId), tag: row.tag, spentAmount: Money.fromCents(row.spentAmount) }),
      onDataChange,
      'watchCategoryTagSpending'
    );
  };

  return {
    watchCategorySpending: watchCategorySpending,
    watchTagSpending: watchTagSpending,
    watchCategoryTagSpending: watchCategoryTagSpending,
  };
};
