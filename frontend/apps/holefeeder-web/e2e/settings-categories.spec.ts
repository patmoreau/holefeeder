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

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withCategories(page);

      await page.goto('/settings/categories');

      await expect(rowOf(page, 'Groceries')).toBeVisible();
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
