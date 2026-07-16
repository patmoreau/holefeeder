import { type AsyncResult, Id, Money, Result } from '@holefeeder/shared/core';
import { AbstractPowerSyncDatabase } from '@powersync/common';
import { CategoriesRepository, CategoriesRepositoryErrors } from '@/flows/core/categories/categories-repository';
import { Category } from '@/flows/core/categories/category';
import { CreateCategoryCommand } from '@/flows/core/categories/create/create-category-command';
import { UpdateCategoryCommand } from '@/flows/core/categories/update/update-category-command';
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
        SELECT id, type, name, color, budget_amount as budgetAmount, favorite, system FROM categories WHERE inactive IS NOT 1 ORDER BY favorite DESC, name
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

  const create = async (command: CreateCategoryCommand): Promise<Result<Id>> => {
    try {
      const id = Id.newId();
      await db.writeTransaction(async (tx) => {
        await tx.execute(
          `INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, inactive)
           VALUES (?, ?, ?, ?, ?, ?, 0, 0)`,
          [id, command.type, command.name, command.color, Money.toCents(command.budgetAmount), command.favorite ? 1 : 0]
        );
      });
      return Result.success(id);
    } catch (error) {
      return Result.failure([String(error)]);
    }
  };

  const update = async (command: UpdateCategoryCommand): Promise<Result<Id>> => {
    try {
      let rowsAffected = 0;
      await db.writeTransaction(async (tx) => {
        const result = await tx.execute(
          `UPDATE categories
           SET type = ?, name = ?, color = ?, budget_amount = ?, favorite = ?
           WHERE id = ? AND system = 0
           RETURNING *`,
          [command.type, command.name, command.color, Money.toCents(command.budgetAmount), command.favorite ? 1 : 0, command.id]
        );
        rowsAffected = result.rows?.length ?? 0;
      });
      if (rowsAffected === 0) return Result.failure([CategoriesRepositoryErrors.categoryNotFound]);
      return Result.success(command.id);
    } catch (error) {
      return Result.failure([String(error)]);
    }
  };

  const deactivate = async (id: Id): Promise<Result<void>> => {
    try {
      let rowsAffected = 0;
      await db.writeTransaction(async (tx) => {
        const result = await tx.execute(
          `UPDATE categories
           SET inactive = 1
           WHERE id = ? AND system = 0
           RETURNING *`,
          [id]
        );
        rowsAffected = result.rows?.length ?? 0;
      });
      if (rowsAffected === 0) return Result.failure([CategoriesRepositoryErrors.categoryNotFound]);
      return Result.success();
    } catch (error) {
      return Result.failure([String(error)]);
    }
  };

  return {
    watch: watch,
    create: create,
    update: update,
    deactivate: deactivate,
  };
};
