import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

type SettingsJson = { effectiveDate: string; intervalType: string; frequency: number };

const withSettings = (page: Page, settings: SettingsJson) =>
  page.route('**/gateway/api/v2/settings', (route) => route.fulfill({ json: settings }));

const monthly: SettingsJson = { effectiveDate: '2026-09-01', intervalType: 'monthly', frequency: 1 };

test.describe('Settings budget', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldSummarizeAMonthlyBudget', async ({ page }) => {
    await withSettings(page, monthly);

    await page.goto('/settings');

    await expect(page.getByTestId('settings-budget')).toContainText('Budget');
    await expect(page.getByTestId('settings-budget-summary')).toHaveText('Monthly · from Sep 1, 2026');
  });

  test('ShouldSummarizeABudgetOverSeveralIntervals', async ({ page }) => {
    await withSettings(page, { effectiveDate: '2026-09-07', intervalType: 'weekly', frequency: 2 });

    await page.goto('/settings');

    await expect(page.getByTestId('settings-budget-summary')).toHaveText('Every 2 week(s) · from Sep 7, 2026');
  });

  test('ShouldReportSettingsThatFailToLoad', async ({ page }) => {
    await page.route('**/gateway/api/v2/settings', (route) => route.fulfill({ status: 500 }));

    await page.goto('/settings');

    await expect(page.getByTestId('settings-budget-summary')).toHaveText('Error');
    await expect(page.getByTestId('settings-budget')).toHaveAttribute('aria-disabled', 'true');
  });

  test('ShouldOpenTheBudgetFlyoutWithTheCurrentSettings', async ({ page }) => {
    await withSettings(page, monthly);
    await page.goto('/settings');

    await page.getByTestId('settings-budget').click();

    const flyout = page.getByTestId('budget-settings');
    await expect(flyout.getByRole('heading', { name: 'Budget Settings' })).toBeVisible();
    await expect(page.getByTestId('budget-settings-effective-date')).toHaveValue('2026-09-01');
    await expect(page.getByTestId('budget-settings-interval')).toHaveText('Monthly');
    await expect(page.getByTestId('budget-settings-frequency')).toHaveValue('1');
    await expect(flyout).toContainText('Each budget period starts on the effective date and repeats at the chosen interval.');
  });

  test('ShouldSaveTheBudgetSettings', async ({ page }) => {
    let saved: SettingsJson = monthly;
    const bodies: unknown[] = [];
    await page.route('**/gateway/api/v2/settings', (route) => route.fulfill({ json: saved }));
    await page.route('**/gateway/api/v2/settings/save-settings', async (route) => {
      bodies.push(route.request().postDataJSON());
      saved = { effectiveDate: '2026-09-07', intervalType: 'weekly', frequency: 2 };
      await route.fulfill({ status: 204 });
    });
    await page.goto('/settings');
    await page.getByTestId('settings-budget').click();

    await page.getByTestId('budget-settings-effective-date').fill('2026-09-07');
    await page.getByTestId('budget-settings-interval').click();
    await page.getByRole('option', { name: 'Weekly' }).click();
    await page.getByTestId('budget-settings-frequency').fill('2');
    await page.getByTestId('budget-settings-save').click();

    await expect(page.getByTestId('budget-settings')).toBeHidden();
    await expect(page.getByTestId('dashboard-toast')).toHaveText('Budget settings saved');
    await expect(page.getByTestId('settings-budget-summary')).toHaveText('Every 2 week(s) · from Sep 7, 2026');
    expect(bodies).toEqual([{ effectiveDate: '2026-09-07', intervalType: 'weekly', frequency: 2 }]);
  });

  test('ShouldKeepTheFlyoutOpenWhenSavingFails', async ({ page }) => {
    await withSettings(page, monthly);
    await page.route('**/gateway/api/v2/settings/save-settings', (route) => route.fulfill({ status: 500 }));
    await page.goto('/settings');
    await page.getByTestId('settings-budget').click();

    await page.getByTestId('budget-settings-save').click();

    await expect(page.getByTestId('budget-settings-error')).toHaveText('Budget settings could not be saved. Try again.');
    await expect(page.getByTestId('budget-settings')).toBeVisible();
  });

  test('ShouldOnlySaveAWholeFrequencyOfAtLeastOne', async ({ page }) => {
    await withSettings(page, monthly);
    await page.goto('/settings');
    await page.getByTestId('settings-budget').click();

    await page.getByTestId('budget-settings-frequency').fill('0');

    await expect(page.getByTestId('budget-settings-save')).toBeDisabled();
  });

  test('ShouldCloseTheFlyoutWithoutSaving', async ({ page }) => {
    await withSettings(page, monthly);
    let posts = 0;
    await page.route('**/gateway/api/v2/settings/save-settings', (route) => {
      posts += 1;
      return route.fulfill({ status: 204 });
    });
    await page.goto('/settings');
    await page.getByTestId('settings-budget').click();

    await page.getByTestId('budget-settings-cancel').click();

    await expect(page.getByTestId('budget-settings')).toBeHidden();
    expect(posts).toBe(0);
  });
});
