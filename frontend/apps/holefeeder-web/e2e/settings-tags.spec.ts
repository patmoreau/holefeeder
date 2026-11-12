import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

type TagJson = { tag: string; transactionCount: number; activeCashflowCount: number };

const listRoute = '**/gateway/api/v2/tags/manage';

const car: TagJson = { tag: 'car', transactionCount: 4, activeCashflowCount: 0 };
const groceries: TagJson = { tag: 'groceries', transactionCount: 1, activeCashflowCount: 0 };
const home: TagJson = { tag: 'home', transactionCount: 12, activeCashflowCount: 1 };
const rent: TagJson = { tag: 'rent', transactionCount: 0, activeCashflowCount: 2 };

const withTags = async (page: Page, tags: TagJson[] = [car, groceries, home, rent]) => {
  let calls = 0;
  await page.route(listRoute, (route) => {
    calls += 1;
    return route.fulfill({ json: tags });
  });
  return () => calls;
};

const rowOf = (page: Page, tag: string) =>
  page.getByTestId('tags-row').filter({ has: page.getByTestId('tags-row-name').getByText(tag, { exact: true }) });

test.describe('Settings tags', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldOpenFromTheSettings', async ({ page }) => {
    await withTags(page);
    await page.route('**/gateway/api/v2/settings', (route) =>
      route.fulfill({ json: { effectiveDate: '2026-09-01', intervalType: 'monthly', frequency: 1 } })
    );
    await page.goto('/settings');

    await page.getByTestId('settings-tags').click();

    await expect(page).toHaveURL(/\/settings\/tags$/);
    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Tags');
    await expect(page.getByTestId('nav-settings')).toHaveAttribute('aria-current', 'page');
  });

  test('ShouldGoBackToTheSettings', async ({ page }) => {
    await withTags(page);
    await page.goto('/settings/tags');

    await page.getByTestId('tags-back').click();

    await expect(page).toHaveURL(/\/settings$/);
  });

  test('ShouldListTheTagsInTheOrderReceived', async ({ page }) => {
    await withTags(page);

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-row-name')).toHaveText(['car', 'groceries', 'home', 'rent']);
  });

  test('ShouldShowHowMuchEachTagIsUsed', async ({ page }) => {
    await withTags(page);

    await page.goto('/settings/tags');

    await expect(rowOf(page, 'home').getByTestId('tags-row-usage')).toHaveText('12 transactions · 1 cashflow');
    await expect(rowOf(page, 'car').getByTestId('tags-row-usage')).toHaveText('4 transactions');
    await expect(rowOf(page, 'groceries').getByTestId('tags-row-usage')).toHaveText('1 transaction');
    await expect(rowOf(page, 'rent').getByTestId('tags-row-usage')).toHaveText('2 cashflows');
  });

  test('ShouldExplainWhereTagsComeFrom_WhenThereAreNone', async ({ page }) => {
    await withTags(page, []);

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-empty')).toContainText('No tags yet');
    await expect(page.getByTestId('tags-empty')).toContainText('Tags you add to expenses show up here, ready to rename or remove.');
  });

  test('ShouldShowTheTagsLoading', async ({ page }) => {
    await page.route(listRoute, () => undefined);

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-loading')).toBeVisible();
  });

  test('ShouldReportTagsThatFailToLoad', async ({ page }) => {
    await page.route(listRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-load-error')).toHaveText('The tags could not be loaded.');
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withTags(page, [{ tag: 'a-very-long-tag-name-that-keeps-going-and-going', transactionCount: 1234, activeCashflowCount: 12 }]);

      await page.goto('/settings/tags');

      await expect(page.getByTestId('tags-row')).toBeVisible();
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
