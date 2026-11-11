import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const aFlow = (title: string) => ({
  id: crypto.randomUUID(),
  cashflowId: crypto.randomUUID(),
  date: '2026-11-14',
  title: title,
  category: 'Groceries and household supplies',
  categoryColor: '#34C759',
  tags: ['weekly', 'family'],
  amount: 1450,
});

const withDashboard = async (page: Page) => {
  await page.route('**/gateway/api/v2/dashboard/summary', (route) =>
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
  await page.route('**/gateway/api/v2/dashboard/accounts', (route) =>
    route.fulfill({
      json: ['Chequing', 'Amex', 'Savings'].map((name) => ({
        id: crypto.randomUUID(),
        name: name,
        caption: 'availableBalance',
        balance: 1986.41,
        projectedBalance: 2550,
        updated: '2026-11-08',
      })),
    })
  );
  await page.route('**/gateway/api/v2/dashboard/transactions', (route) =>
    route.fulfill({ json: [aFlow('Groceries at the market down the street')] })
  );
  await page.route('**/gateway/api/v2/dashboard/upcoming', (route) => route.fulfill({ json: [aFlow('Rent for the downtown apartment')] }));
};

test.describe('Phone', () => {
  test.use({ viewport: { width: 360, height: 780 } });

  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    await withDashboard(page);
  });

  test('ShouldHideNavigationBehindMenuButton', async ({ page }) => {
    await page.goto('/');

    await expect(page.getByTestId('nav-menu')).toBeVisible();
    await expect(page.getByTestId('nav-menu')).toHaveAccessibleName('Open menu');
    await expect(page.getByRole('navigation')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-page')).toBeVisible();
  });

  test('ShouldOpenNavigationFromMenuButton', async ({ page }) => {
    await page.goto('/');

    await page.getByTestId('nav-menu').click();

    const nav = page.getByRole('navigation');
    await expect(nav.getByTestId('nav-dashboard')).toHaveAttribute('aria-current', 'page');
    await expect(nav.getByTestId('nav-legacy')).toBeVisible();
    await expect(nav.getByTestId('nav-version')).toHaveText('1.2.3 · abcdef12');
  });

  test('ShouldCloseNavigationWhenAnItemIsChosen', async ({ page }) => {
    await page.goto('/');
    await page.getByTestId('nav-menu').click();

    await page.getByRole('navigation').getByTestId('nav-dashboard').click();

    await expect(page.getByRole('navigation')).toHaveCount(0);
  });

  test('ShouldCloseNavigationWithEscape', async ({ page }) => {
    await page.goto('/');
    await page.getByTestId('nav-menu').click();
    await expect(page.getByRole('navigation')).toBeVisible();

    await page.keyboard.press('Escape');

    await expect(page.getByRole('navigation')).toHaveCount(0);
  });

  test('ShouldNotScrollSideways', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByTestId('dashboard-upcoming-row')).toBeVisible();

    const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);

    expect(overflow).toBe(0);
  });

  test('ShouldStackSpendingTotalsBelowCurrentSpending', async ({ page }) => {
    await page.goto('/');

    const spending = await page.getByTestId('dashboard-current-spending').boundingBox();
    const projected = await page.getByTestId('dashboard-projected').boundingBox();
    expect(projected!.y).toBeGreaterThan(spending!.y + spending!.height);
  });

  test('ShouldStackUpcomingBelowRecentTransactions', async ({ page }) => {
    await page.goto('/');

    const transaction = await page.getByTestId('dashboard-transaction-row').boundingBox();
    const upcoming = await page.getByTestId('dashboard-upcoming-row').boundingBox();
    expect(upcoming!.y).toBeGreaterThan(transaction!.y + transaction!.height);
  });

  test('ShouldSwipeAccountsWithoutArrows', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByTestId('dashboard-account-name')).toHaveCount(3);

    await expect(page.getByTestId('dashboard-accounts-next')).toBeHidden();
  });

  test('ShouldKeepNewTransactionBesideTitle', async ({ page }) => {
    await page.goto('/');

    const heading = await page.getByRole('heading', { name: 'Dashboard' }).boundingBox();
    const button = await page.getByTestId('dashboard-new-transaction').boundingBox();
    expect(button!.x).toBeGreaterThan(heading!.x + heading!.width);
    expect(button!.height).toBeLessThan(48);
  });
});

test.describe('Desktop', () => {
  test('ShouldHideMenuButton', async ({ page }) => {
    await signedIn(page);

    await page.goto('/');

    await expect(page.getByRole('navigation').getByTestId('nav-dashboard')).toBeVisible();
    await expect(page.getByTestId('nav-menu')).toBeHidden();
  });
});
