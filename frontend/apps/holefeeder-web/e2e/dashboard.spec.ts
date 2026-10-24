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

  test('ShouldOpenLegacyAppInNewTab', async ({ page }) => {
    await signedIn(page);

    await page.goto('/');

    const legacy = page.getByRole('navigation').getByTestId('nav-legacy');
    await expect(legacy).toHaveText('Legacy');
    await expect(legacy).toHaveAttribute('href', '/angular/');
    await expect(legacy).toHaveAttribute('target', '_blank');
    await expect(legacy).toHaveAttribute('rel', 'noopener noreferrer');
  });

  test('ShouldShowSkeletonOfEachSection', async ({ page }) => {
    await signedIn(page);

    await page.goto('/');

    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible();
    await expect(page.getByTestId('dashboard-spending-skeleton')).toBeVisible();
    await expect(page.getByTestId('dashboard-account-skeleton')).toHaveCount(4);
    await expect(page.getByTestId('dashboard-transactions-skeleton')).toBeVisible();
    await expect(page.getByTestId('dashboard-upcoming-skeleton')).toBeVisible();
  });
});
