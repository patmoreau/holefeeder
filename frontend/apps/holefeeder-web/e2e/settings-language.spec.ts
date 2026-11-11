import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const withSpending = (page: Page) =>
  page.route('**/gateway/api/v2/dashboard/summary', (route) =>
    route.fulfill({
      json: {
        currentExpenses: 1847.63,
        expenseVariation: -212.4,
        expenseVariationPercentage: -10.31,
        netFlow: 842.17,
        currentGains: 2689.8,
        averageExpenses: 2060.03,
        projectedNetFlow: 317.92,
      },
    })
  );

test.describe('Settings language', () => {
  test.describe('in an English browser', () => {
    test.use({ locale: 'en-CA' });

    test('ShouldShowEnglishByDefault', async ({ page }) => {
      await signedIn(page);

      await page.goto('/settings');

      await expect(page.getByTestId('settings-language')).toHaveText('English');
      await expect(page.locator('html')).toHaveAttribute('lang', 'en');
    });

    test('ShouldKeepChosenLanguageInTheBrowser', async ({ page }) => {
      await signedIn(page);
      await page.goto('/settings');

      await page.getByTestId('settings-language').click();
      await page.getByRole('option', { name: 'French' }).click();

      await expect(page.getByRole('heading', { name: 'Configuration' })).toBeVisible();
      await expect(page.getByTestId('settings-language')).toHaveText('Français');
      await page.reload();
      await expect(page.getByRole('heading', { name: 'Configuration' })).toBeVisible();
      await expect(page.locator('html')).toHaveAttribute('lang', 'fr');
    });
  });

  test.describe('in a French browser', () => {
    test.use({ locale: 'fr-CA' });

    test('ShouldFollowTheBrowserLanguage', async ({ page }) => {
      await signedIn(page);

      await page.goto('/settings');

      await expect(page.getByRole('heading', { name: 'Configuration' })).toBeVisible();
      await expect(page.getByTestId('settings-language')).toHaveText('Français');
      await expect(page.getByTestId('settings-logout')).toHaveText('Déconnexion');
    });

    test('ShouldFormatAmountsTheFrenchWay', async ({ page }) => {
      await signedIn(page);
      await withSpending(page);

      await page.goto('/');

      await expect(page.getByTestId('dashboard-current-spending')).toHaveText(/^1\s847,63\s\$$/);
    });
  });
});
