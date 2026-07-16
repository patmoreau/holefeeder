import type { AsyncResult } from '@holefeeder/shared/core';
import { waitFor } from '@testing-library/react-native';
import { aCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { CategoriesRepositoryErrors } from '@/flows/core/categories/categories-repository';
import { Category } from '@/flows/core/categories/category';
import { aCreateCategoryCommand } from '@/flows/core/categories/create/__tests__/create-category-command-for-test';
import { anUpdateCategoryCommand } from '@/flows/core/categories/update/__tests__/update-category-command-for-test';
import { anId } from '@/shared/__tests__/string-for-test';
import { System } from '@/shared/core/system';
import { DatabaseForTest, setupDatabaseForTest } from '@/shared/persistence/__tests__/database-for-test';
import { CategoriesRepositoryInPowersync } from './categories-repository-in-powersync';

const watchCategories = (repo: ReturnType<typeof CategoriesRepositoryInPowersync>): Promise<Category[]> =>
  new Promise((resolve) => {
    let result: AsyncResult<Category[]> | undefined;
    const unsubscribe = repo.watch((data) => {
      result = data;
    });
    waitFor(() => expect(result).toBeDefined()).then(() => {
      unsubscribe();
      resolve(result!.isSuccess ? result!.value : []);
    });
  });

describe('CategoriesRepositoryInPowersync', () => {
  let db: DatabaseForTest;

  beforeEach(async () => {
    db = await setupDatabaseForTest();
  });

  afterEach(async () => {
    await db.cleanupTestDb();
  });

  describe('watch', () => {
    it('retrieves a stored category', async () => {
      const category = await aCategory().store(db);
      const repo = CategoriesRepositoryInPowersync(db);

      let result: AsyncResult<unknown> | undefined;
      const unsubscribe = repo.watch((data) => {
        result = data;
      });

      await waitFor(() => {
        expect(result).toBeDefined();
      });

      expect(result).toBeSuccessWithValue([
        {
          id: category.id,
          type: category.type,
          name: category.name,
          color: category.color,
          budgetAmount: category.budgetAmount,
          favorite: category.favorite,
          system: category.system,
        },
      ]);

      unsubscribe();
    });

    it('includes categories with a null inactive flag', async () => {
      const category = aCategory({ system: false as System });
      await db.execute(
        'INSERT INTO categories (id, type, name, color, budget_amount, favorite, system, inactive, user_id) VALUES (?, ?, ?, ?, ?, ?, ?, NULL, ?)',
        [category.id, category.type, category.name, category.color, 0, 0, 0, anId()]
      );
      const repo = CategoriesRepositoryInPowersync(db);

      const categories = await watchCategories(repo);

      expect(categories.map((c) => c.id)).toEqual([category.id]);
    });

    it('returns not found when no categories exist', async () => {
      const repo = CategoriesRepositoryInPowersync(db);

      let result: AsyncResult<unknown> | undefined;
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
      const repo = CategoriesRepositoryInPowersync(db);

      // Close the database to trigger an error
      await db.close();

      let result: AsyncResult<unknown> | undefined;
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

  describe('create', () => {
    it('inserts a user category', async () => {
      const repo = CategoriesRepositoryInPowersync(db);
      const command = aCreateCategoryCommand({ name: 'Groceries' });

      const result = await repo.create(command);

      expect(result.isSuccess).toBe(true);

      const categories = await watchCategories(repo);
      expect(categories).toHaveLength(1);
      expect(categories[0].name).toBe('Groceries');
      expect(categories[0].system).toBe(false as System);
    });

    it('returns failure on database error', async () => {
      const repo = CategoriesRepositoryInPowersync(db);
      await db.close();

      const result = await repo.create(aCreateCategoryCommand());

      expect(result.isFailure).toBe(true);
    });
  });

  describe('update', () => {
    it('updates an existing user category', async () => {
      const category = await aCategory({ system: false as System }).store(db);
      const repo = CategoriesRepositoryInPowersync(db);
      const command = anUpdateCategoryCommand({ id: category.id, name: 'New Name' });

      const result = await repo.update(command);

      expect(result).toBeSuccessWithValue(category.id);

      const categories = await watchCategories(repo);
      expect(categories[0].name).toBe('New Name');
    });

    it('does not update a system category', async () => {
      const category = await aCategory({ system: true as System }).store(db);
      const repo = CategoriesRepositoryInPowersync(db);
      const command = anUpdateCategoryCommand({ id: category.id, name: 'Hacked' });

      const result = await repo.update(command);

      expect(result).toBeFailureWithErrors([CategoriesRepositoryErrors.categoryNotFound]);
    });

    it('returns failure when category does not exist', async () => {
      const repo = CategoriesRepositoryInPowersync(db);

      const result = await repo.update(anUpdateCategoryCommand());

      expect(result).toBeFailureWithErrors([CategoriesRepositoryErrors.categoryNotFound]);
    });

    it('returns failure on database error', async () => {
      const repo = CategoriesRepositoryInPowersync(db);
      await db.close();

      const result = await repo.update(anUpdateCategoryCommand());

      expect(result.isFailure).toBe(true);
    });
  });

  describe('deactivate', () => {
    it('soft deletes a user category', async () => {
      const category = await aCategory({ system: false as System }).store(db);
      const repo = CategoriesRepositoryInPowersync(db);

      const result = await repo.deactivate(category.id);

      expect(result.isSuccess).toBe(true);
      const rows = await db.getAll<{ inactive: number }>('SELECT inactive FROM categories WHERE id = ?', [category.id]);
      expect(rows[0].inactive).toBe(1);
    });

    it('excludes a deactivated category from watch', async () => {
      const category = await aCategory({ system: false as System }).store(db);
      const repo = CategoriesRepositoryInPowersync(db);

      await repo.deactivate(category.id);

      const categories = await watchCategories(repo);
      expect(categories).toEqual([]);
    });

    it('does not deactivate a system category', async () => {
      const category = await aCategory({ system: true as System }).store(db);
      const repo = CategoriesRepositoryInPowersync(db);

      const result = await repo.deactivate(category.id);

      expect(result).toBeFailureWithErrors([CategoriesRepositoryErrors.categoryNotFound]);
      const rows = await db.getAll<{ inactive: number }>('SELECT inactive FROM categories WHERE id = ?', [category.id]);
      expect(rows[0].inactive).toBe(0);
    });

    it('returns failure when category does not exist', async () => {
      const repo = CategoriesRepositoryInPowersync(db);

      const result = await repo.deactivate(anId());

      expect(result).toBeFailureWithErrors([CategoriesRepositoryErrors.categoryNotFound]);
    });

    it('returns failure on database error', async () => {
      const category = await aCategory({ system: false as System }).store(db);
      const repo = CategoriesRepositoryInPowersync(db);
      await db.close();

      const result = await repo.deactivate(category.id);

      expect(result.isFailure).toBe(true);
    });
  });
});
