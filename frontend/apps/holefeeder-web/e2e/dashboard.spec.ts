import { expect, test } from '@playwright/test';
import { signedIn, signedOut } from './support/auth';

test.describe('Dashboard', () => {
  test('ShouldSendSignedOutVisitorToWelcome', async ({ page }) => {
    await signedOut(page);

    await page.goto('/');

    await expect(page).toHaveURL('/welcome');
    await expect(page.getByTestId('dashboard-page')).toHaveCount(0);
  });

  test('ShouldShowNavigationWithDashboardSelected', async ({ page }) => {
    await signedIn(page);

    await page.goto('/');

    const nav = page.getByRole('navigation');
    await expect(nav.getByTestId('nav-brand-tile')).toBeVisible();
    await expect(nav.getByTestId('nav-dashboard')).toHaveAttribute('aria-current', 'page');
    await expect(nav.getByTestId('nav-dashboard')).toHaveText('Dashboard');
    await expect(nav.getByTestId('nav-insights')).toHaveText('Insights');
    await expect(nav.getByTestId('nav-settings')).toHaveText('Settings');
  });

  test('ShouldNotLinkToLegacyApp', async ({ page }) => {
    await signedIn(page);

    await page.goto('/');

    await expect(page.getByRole('navigation').getByTestId('nav-settings')).toBeVisible();
    await expect(page.locator('a[href^="/angular"]')).toHaveCount(0);
  });

  test('ShouldShowBuildVersionAtBottomOfNavigation', async ({ page }) => {
    await signedIn(page);

    await page.goto('/');

    const nav = page.getByRole('navigation');
    await expect(nav.getByTestId('nav-version')).toHaveText('1.2.3 · abcdef12');
    const settings = await nav.getByTestId('nav-settings').boundingBox();
    const version = await nav.getByTestId('nav-version').boundingBox();
    const drawer = await nav.boundingBox();
    expect(version!.y).toBeGreaterThan(settings!.y + settings!.height + 100);
    expect(drawer!.y + drawer!.height - (version!.y + version!.height)).toBeLessThan(40);
  });

  test('ShouldShowNewTransactionButtonInTopBar', async ({ page }) => {
    await signedIn(page);

    await page.goto('/');

    const heading = page.getByRole('heading', { name: 'Dashboard' });
    const button = page.getByTestId('dashboard-new-transaction');
    await expect(button).toHaveAccessibleName('New transaction');
    await expect(button).toBeEnabled();
    const headingBox = await heading.boundingBox();
    const buttonBox = await button.boundingBox();
    expect(buttonBox!.x).toBeGreaterThan(headingBox!.x + headingBox!.width);
    expect(Math.abs(buttonBox!.y + buttonBox!.height / 2 - (headingBox!.y + headingBox!.height / 2))).toBeLessThan(2);
  });

  test('ShouldShowSkeletonOfEachSection', async ({ page }) => {
    await signedIn(page);
    await page.route('**/gateway/api/v2/dashboard/summary', () => new Promise<void>(() => {}));
    await page.route('**/gateway/api/v2/dashboard/accounts', () => new Promise<void>(() => {}));
    await page.route('**/gateway/api/v2/dashboard/transactions', () => new Promise<void>(() => {}));
    await page.route('**/gateway/api/v2/dashboard/upcoming', () => new Promise<void>(() => {}));

    await page.goto('/');

    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible();
    await expect(page.getByTestId('dashboard-spending-skeleton')).toBeVisible();
    await expect(page.getByTestId('dashboard-account-skeleton')).toHaveCount(4);
    await expect(page.getByTestId('dashboard-transactions-skeleton')).toBeVisible();
    await expect(page.getByTestId('dashboard-upcoming-skeleton')).toBeVisible();
  });
});
