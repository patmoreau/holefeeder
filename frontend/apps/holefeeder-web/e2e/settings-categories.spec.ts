import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

type CategoryJson = {
  id: string;
  name: string;
  type: 'expense' | 'gain';
  color: string;
  budgetAmount: number;
  favorite: boolean;
  system: boolean;
};

const listRoute = '**/gateway/api/v2/categories/manage';

const groceries: CategoryJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3311',
  name: 'Groceries',
  type: 'expense',
  color: '#34C759',
  budgetAmount: 600,
  favorite: true,
  system: false,
};
const dining: CategoryJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3312',
  name: 'Dining',
  type: 'expense',
  color: '#FF9500',
  budgetAmount: 0,
  favorite: false,
  system: false,
};
const salary: CategoryJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3313',
  name: 'Salary',
  type: 'gain',
  color: '#30B0C7',
  budgetAmount: 0,
  favorite: true,
  system: false,
};
const transferIn: CategoryJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3314',
  name: 'Transfer In',
  type: 'gain',
  color: '#607D8B',
  budgetAmount: 0,
  favorite: false,
  system: true,
};

const withCategories = async (page: Page, categories: CategoryJson[] = [groceries, dining, salary, transferIn]) => {
  let calls = 0;
  await page.route(listRoute, (route) => {
    calls += 1;
    return route.fulfill({ json: categories });
  });
  return () => calls;
};

const rowOf = (page: Page, name: string) => page.getByTestId('categories-row').filter({ hasText: name });

const capture = async (page: Page, url: string, status = 204, json?: unknown) => {
  const bodies: unknown[] = [];
  await page.route(url, (route) => {
    bodies.push(route.request().postDataJSON());
    return json === undefined ? route.fulfill({ status: status }) : route.fulfill({ status: status, json: json });
  });
  return bodies;
};

const createRoute = '**/gateway/api/v2/categories';
const modifyRoute = '**/gateway/api/v2/categories/modify';

test.describe('Settings categories', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldOpenFromTheSettings', async ({ page }) => {
    await withCategories(page);
    await page.route('**/gateway/api/v2/settings', (route) =>
      route.fulfill({ json: { effectiveDate: '2026-09-01', intervalType: 'monthly', frequency: 1 } })
    );
    await page.goto('/settings');

    await page.getByTestId('settings-categories').click();

    await expect(page).toHaveURL(/\/settings\/categories$/);
    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Categories');
    await expect(page.getByTestId('nav-settings')).toHaveAttribute('aria-current', 'page');
  });

  test('ShouldGoBackToTheSettings', async ({ page }) => {
    await withCategories(page);
    await page.goto('/settings/categories');

    await page.getByTestId('categories-back').click();

    await expect(page).toHaveURL(/\/settings$/);
  });

  test('ShouldListTheCategoriesBySection', async ({ page }) => {
    await withCategories(page);

    await page.goto('/settings/categories');

    await expect(page.getByTestId('categories-expenses').getByTestId('categories-row')).toHaveText([/Groceries/, /Dining/]);
    await expect(page.getByTestId('categories-income').getByTestId('categories-row')).toHaveText([/Salary/]);
    await expect(page.getByTestId('categories-system').getByTestId('categories-row')).toHaveText([/Transfer In/]);
    const sections = page.locator('[data-testid="categories-expenses"], [data-testid="categories-income"], [data-testid="categories-system"]');
    expect(await sections.evaluateAll((elements) => elements.map((element) => element.getAttribute('data-testid')))).toEqual([
      'categories-expenses',
      'categories-income',
      'categories-system',
    ]);
  });

  test('ShouldShowTheFavoriteAndTheBudget', async ({ page }) => {
    await withCategories(page);

    await page.goto('/settings/categories');

    await expect(rowOf(page, 'Groceries').getByLabel('Favorite')).toBeVisible();
    await expect(rowOf(page, 'Groceries').getByTestId('categories-row-budget')).toHaveText('$600.00');
    await expect(rowOf(page, 'Dining').getByLabel('Favorite')).toHaveCount(0);
    await expect(rowOf(page, 'Dining').getByTestId('categories-row-budget')).toHaveCount(0);
  });

  test('ShouldMarkTheSystemCategories', async ({ page }) => {
    await withCategories(page);

    await page.goto('/settings/categories');

    await expect(rowOf(page, 'Transfer In')).toContainText('System');
    await expect(rowOf(page, 'Transfer In').getByRole('button')).toHaveCount(0);
  });

  test('ShouldLeaveOutEmptySections', async ({ page }) => {
    await withCategories(page, [dining, transferIn]);

    await page.goto('/settings/categories');

    await expect(page.getByTestId('categories-expenses')).toBeVisible();
    await expect(page.getByTestId('categories-income')).toHaveCount(0);
  });

  test('ShouldInviteToAddACategory_WhenThereAreNone', async ({ page }) => {
    await withCategories(page, [transferIn]);

    await page.goto('/settings/categories');

    await expect(page.getByTestId('categories-empty')).toContainText('No categories yet');
  });

  test('ShouldReportCategoriesThatFailToLoad', async ({ page }) => {
    await page.route(listRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto('/settings/categories');

    await expect(page.getByTestId('categories-load-error')).toHaveText('The categories could not be loaded.');
  });

  test('ShouldOpenAnEmptyNewCategory', async ({ page }) => {
    await withCategories(page);
    await page.goto('/settings/categories');

    await page.getByTestId('categories-new').click();

    const flyout = page.getByTestId('category');
    await expect(flyout.getByRole('heading')).toHaveText('New category');
    await expect(flyout.getByRole('button', { name: 'Expense' })).toHaveAttribute('aria-pressed', 'true');
    await expect(flyout.getByTestId('category-name')).toHaveValue('');
    await expect(flyout.getByTestId('category-budget')).toHaveValue('0.00');
    await expect(flyout.getByRole('radio', { checked: true })).toHaveAccessibleName('#34C759');
    await expect(flyout.getByTestId('category-favorite')).not.toBeChecked();
    await expect(flyout.getByTestId('category-save')).toBeDisabled();
  });

  test('ShouldCreateTheCategory', async ({ page }) => {
    const listCalls = await withCategories(page);
    const bodies = await capture(page, createRoute, 201, { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3399' });
    await page.goto('/settings/categories');
    const callsBefore = listCalls();

    await page.getByTestId('categories-new').click();
    const flyout = page.getByTestId('category');
    await flyout.getByRole('button', { name: 'Income' }).click();
    await flyout.getByTestId('category-name').fill('Bonus');
    await flyout.getByTestId('category-budget').fill('50');
    await flyout.getByRole('radio', { name: '#AF52DE' }).click();
    await flyout.getByTestId('category-favorite').check();
    await flyout.getByTestId('category-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Category saved');
    await expect(page.getByTestId('category')).toHaveCount(0);
    expect(bodies).toEqual([{ name: 'Bonus', type: 'gain', color: '#AF52DE', budgetAmount: '50.00', favorite: true }]);
    await expect.poll(listCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldOnlySaveANamedCategoryWithAValidBudget', async ({ page }) => {
    await withCategories(page);
    await page.goto('/settings/categories');
    await page.getByTestId('categories-new').click();
    const flyout = page.getByTestId('category');

    await flyout.getByTestId('category-name').fill('   ');
    await expect(flyout.getByTestId('category-save')).toBeDisabled();
    await flyout.getByTestId('category-name').fill('Bonus');
    await flyout.getByTestId('category-budget').fill('12.345');
    await expect(flyout.getByTestId('category-save')).toBeDisabled();
    await flyout.getByTestId('category-budget').fill('12.34');
    await expect(flyout.getByTestId('category-save')).toBeEnabled();
  });

  test('ShouldReportANameAlreadyUsed', async ({ page }) => {
    await withCategories(page);
    await capture(page, createRoute, 400, { detail: 'Category name is already used.', code: 'category-name-taken' });
    await page.goto('/settings/categories');
    await page.getByTestId('categories-new').click();
    const flyout = page.getByTestId('category');

    await flyout.getByTestId('category-name').fill('Dining');
    await flyout.getByTestId('category-save').click();

    await expect(flyout.getByTestId('category-error')).toHaveText('That name is already used.');
    await expect(flyout).toBeVisible();
  });

  test('ShouldKeepTheFlyoutOpenWhenSavingFails', async ({ page }) => {
    await withCategories(page);
    await capture(page, createRoute, 500);
    await page.goto('/settings/categories');
    await page.getByTestId('categories-new').click();
    const flyout = page.getByTestId('category');

    await flyout.getByTestId('category-name').fill('Bonus');
    await flyout.getByTestId('category-save').click();

    await expect(flyout.getByTestId('category-error')).toHaveText('The category could not be saved. Try again.');
  });

  test('ShouldOpenTheCategoryFromItsRow', async ({ page }) => {
    await withCategories(page);
    await page.goto('/settings/categories');

    await rowOf(page, 'Groceries').getByTestId('categories-row-open').click();

    const flyout = page.getByTestId('category');
    await expect(flyout.getByRole('heading')).toHaveText('Edit category');
    await expect(flyout.getByRole('button', { name: 'Expense' })).toHaveAttribute('aria-pressed', 'true');
    await expect(flyout.getByTestId('category-name')).toHaveValue('Groceries');
    await expect(flyout.getByTestId('category-budget')).toHaveValue('600.00');
    await expect(flyout.getByRole('radio', { checked: true })).toHaveAccessibleName('#34C759');
    await expect(flyout.getByTestId('category-favorite')).toBeChecked();
  });

  test('ShouldSaveTheChanges', async ({ page }) => {
    const listCalls = await withCategories(page);
    const bodies = await capture(page, modifyRoute);
    await page.goto('/settings/categories');
    const callsBefore = listCalls();

    await rowOf(page, 'Groceries').getByTestId('categories-row-open').click();
    const flyout = page.getByTestId('category');
    await flyout.getByTestId('category-name').fill('Food');
    await flyout.getByTestId('category-budget').fill('0');
    await flyout.getByTestId('category-favorite').uncheck();
    await flyout.getByTestId('category-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Category saved');
    expect(bodies).toEqual([{ id: groceries.id, name: 'Food', type: 'expense', color: '#34C759', budgetAmount: '0.00', favorite: false }]);
    await expect.poll(listCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldKeepAColorOutsideTheSwatches', async ({ page }) => {
    await withCategories(page, [{ ...dining, color: '#123456' }]);
    await page.goto('/settings/categories');

    await rowOf(page, 'Dining').getByTestId('categories-row-open').click();

    await expect(page.getByTestId('category').getByRole('radio', { checked: true })).toHaveAccessibleName('#123456');
  });

  test('ShouldCloseWithoutSaving', async ({ page }) => {
    await withCategories(page);
    const bodies = await capture(page, modifyRoute);
    await page.goto('/settings/categories');

    await rowOf(page, 'Groceries').getByTestId('categories-row-open').click();
    await page.getByTestId('category-cancel').click();

    await expect(page.getByTestId('category')).toHaveCount(0);
    expect(bodies).toEqual([]);
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withCategories(page);

      await page.goto('/settings/categories');

      await expect(rowOf(page, 'Groceries')).toBeVisible();
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });

    test('ShouldOpenTheFlyoutFullWidth', async ({ page }) => {
      await withCategories(page);
      await page.goto('/settings/categories');

      await page.getByTestId('categories-new').click();

      const flyout = page.getByTestId('category');
      await expect(flyout.getByTestId('category-name')).toBeVisible();
      await expect.poll(async () => Math.round((await flyout.boundingBox())?.width ?? 0)).toBe(360);
      const overflow = await flyout.evaluate((element) => element.scrollWidth - element.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
