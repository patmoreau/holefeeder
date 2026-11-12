import { expect, test } from '@playwright/test';
import { logoutCalls, signedIn, signedOut } from './support/auth';

test.describe('Settings', () => {
  test('ShouldSendSignedOutVisitorToWelcome', async ({ page }) => {
    await signedOut(page);

    await page.goto('/settings');

    await expect(page).toHaveURL('/welcome');
  });

  test('ShouldOpenFromNavigation', async ({ page }) => {
    await signedIn(page);
    await page.goto('/');

    await page.getByRole('navigation').getByTestId('nav-settings').click();

    await expect(page).toHaveURL('/settings');
    await expect(page.getByRole('heading', { name: 'Settings' })).toBeVisible();
    await expect(page.getByRole('navigation').getByTestId('nav-settings')).toHaveAttribute('aria-current', 'page');
  });

  test('ShouldShowSignedInUser', async ({ page }) => {
    await signedIn(page, { sub: 'auth0|alex', name: 'Alex Martin', email: 'alex.martin@example.com' });

    await page.goto('/settings');

    await expect(page.getByTestId('settings-profile-name')).toHaveText('Alex Martin');
    await expect(page.getByTestId('settings-profile-email')).toHaveText('alex.martin@example.com');
    await expect(page.getByTestId('settings-profile-avatar')).toHaveText('AM');
  });

  test('ShouldLogOut', async ({ page }) => {
    await signedIn(page);
    await page.goto('/settings');

    await page.getByTestId('settings-logout').click();

    expect(await logoutCalls(page)).toBe(1);
  });

  test('ShouldDisableManagementRowsUntilTheirPagesExist', async ({ page }) => {
    await signedIn(page);

    await page.goto('/settings');

    await expect(page.getByTestId('settings-tags')).toHaveAttribute('aria-disabled', 'true');
    for (const row of ['categories', 'cashflows']) {
      await expect(page.getByTestId(`settings-${row}`)).not.toHaveAttribute('aria-disabled', 'true');
    }
    await expect(page.getByTestId('settings-categories')).toHaveText('Categories');
    await expect(page.getByTestId('settings-cashflows')).toHaveText('Cashflows');
    await expect(page.getByTestId('settings-tags')).toHaveText('Tags');
  });

  test('ShouldShowBuildVersion', async ({ page }) => {
    await signedIn(page);

    await page.goto('/settings');

    await expect(page.getByTestId('settings-version')).toHaveText('1.2.3 · abcdef12');
  });

  test('ShouldFollowSystemThemeByDefault', async ({ page }) => {
    await page.emulateMedia({ colorScheme: 'dark' });
    await signedIn(page);

    await page.goto('/settings');

    await expect(page.getByTestId('settings-theme')).toHaveText('System');
    await expect(page.locator('html')).toHaveClass(/dark/);
  });

  test('ShouldKeepChosenThemeInTheBrowser', async ({ page }) => {
    await page.emulateMedia({ colorScheme: 'light' });
    await signedIn(page);
    await page.goto('/settings');

    await page.getByTestId('settings-theme').click();
    await page.getByRole('option', { name: 'Dark' }).click();

    await expect(page.locator('html')).toHaveClass(/dark/);
    await page.reload();
    await expect(page.getByTestId('settings-theme')).toHaveText('Dark');
    await expect(page.locator('html')).toHaveClass(/dark/);
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 780 } });

    test('ShouldFitTheScreenWidth', async ({ page }) => {
      await signedIn(page);

      await page.goto('/settings');

      await expect(page.getByTestId('settings-logout')).toBeVisible();
      const scrollWidth = await page.evaluate(() => document.documentElement.scrollWidth);
      expect(scrollWidth).toBeLessThanOrEqual(360);
      const logout = await page.getByTestId('settings-logout').boundingBox();
      expect(logout!.x + logout!.width).toBeLessThanOrEqual(360 - 16);
    });
  });
});
