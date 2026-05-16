import { AbstractPowerSyncDatabase } from '@powersync/common';
import { CategoriesRepository } from '@/flows/core/categories/categories-repository';
import { Category } from '@/flows/core/categories/category';
import { Money } from '@/shared/core/money';
import { type AsyncResult } from '@/shared/core/result';
import { watchQuery } from '@/shared/persistence/watch-query';

type CategoryRow = {
  id: number;
  type: string;
  name: string;
  color: string;
  budgetAmount: number;
  favorite: number;
  system: number;
};

export const CategoriesRepositoryInPowersync = (db: AbstractPowerSyncDatabase): CategoriesRepository => {
  const watch = (onDataChange: (result: AsyncResult<Category[]>) => void) => {
    return watchQuery<CategoryRow, Category>(
      db,
      `
        SELECT id, type, name, color, budget_amount as budgetAmount, favorite, system FROM categories ORDER BY favorite DESC, name
        `,
      [],
      (row) =>
        Category.valid({
          ...row,
          budgetAmount: Money.fromCents(row.budgetAmount),
          favorite: row.favorite === 1,
          system: row.system === 1,
        }),
      onDataChange,
      'watchCategories'
    );
  };

  return {
    watch: watch,
  };
};
