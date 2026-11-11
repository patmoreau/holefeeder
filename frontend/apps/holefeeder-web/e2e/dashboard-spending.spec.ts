import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const summaryRoute = '**/gateway/api/v2/dashboard/summary';

const positive = 'rgb(46, 125, 50)';
const negative = 'rgb(211, 47, 47)';
const neutral = 'rgb(28, 28, 30)';

const aSummary = (overrides: Record<string, number> = {}) => ({
  currentExpenses: 1847.63,
  expenseVariation: -212.4,
  expenseVariationPercentage: -10.31,
  netFlow: 842.17,
  currentGains: 2689.8,
  averageExpenses: 2060.03,
  projectedNetFlow: 317.92,
  ...overrides,
});

const withSummary = (page: Page, summary: ReturnType<typeof aSummary>) => page.route(summaryRoute, (route) => route.fulfill({ json: summary }));

test.describe('Dashboard spending', () => {
  test('ShouldShowCurrentSpendingFromApiWithBearerToken', async ({ page }) => {
    await signedIn(page);
    let authorization: string | undefined;
    await page.route(summaryRoute, async (route) => {
      authorization = route.request().headers()['authorization'];
      await route.fulfill({ json: aSummary() });
    });

    await page.goto('/');

    const hero = page.getByTestId('dashboard-spending-hero');
    await expect(hero.getByText('Current Spending')).toBeVisible();
    await expect(page.getByTestId('dashboard-current-spending')).toHaveText('$1,847.63');
    expect(authorization).toBe('Bearer e2e-token');
  });

  test('ShouldShowTrendBadgeBelowAverage', async ({ page }) => {
    await signedIn(page);
    await withSummary(page, aSummary());

    await page.goto('/');

    const trend = page.getByTestId('dashboard-spending-trend');
    await expect(trend).toHaveText('↘$212.40 vs Average');
    await expect(trend.getByText('$212.40 vs Average')).toHaveCSS('color', positive);
  });

  test('ShouldShowTrendBadgeAboveAverage', async ({ page }) => {
    await signedIn(page);
    await withSummary(page, aSummary({ expenseVariation: 150.5 }));

    await page.goto('/');

    const trend = page.getByTestId('dashboard-spending-trend');
    await expect(trend).toHaveText('↗$150.50 vs Average');
    await expect(trend.getByText('$150.50 vs Average')).toHaveCSS('color', negative);
  });

  test('ShouldHideTrendBadgeWithoutAverage', async ({ page }) => {
    await signedIn(page);
    await withSummary(page, aSummary({ averageExpenses: 0.004, expenseVariation: 1847.63 }));

    await page.goto('/');

    await expect(page.getByTestId('dashboard-current-spending')).toBeVisible();
    await expect(page.getByTestId('dashboard-spending-trend')).toHaveCount(0);
  });

  test('ShouldShowNetFlowAndProjectedPillsWithTone', async ({ page }) => {
    await signedIn(page);
    await withSummary(page, aSummary({ netFlow: 842.17, projectedNetFlow: -317.92 }));

    await page.goto('/');

    const hero = page.getByTestId('dashboard-spending-hero');
    await expect(hero.getByText('Net Flow')).toBeVisible();
    await expect(hero.getByText('Projected')).toBeVisible();
    await expect(page.getByTestId('dashboard-net-flow')).toHaveText('$842.17');
    await expect(page.getByTestId('dashboard-net-flow')).toHaveCSS('color', positive);
    await expect(page.getByTestId('dashboard-projected')).toHaveText('$317.92');
    await expect(page.getByTestId('dashboard-projected')).toHaveCSS('color', negative);
  });

  test('ShouldShowNeutralPillWhenAmountRoundsToZero', async ({ page }) => {
    await signedIn(page);
    await withSummary(page, aSummary({ netFlow: -0.004, projectedNetFlow: 0 }));

    await page.goto('/');

    await expect(page.getByTestId('dashboard-net-flow')).toHaveText('$0.00');
    await expect(page.getByTestId('dashboard-net-flow')).toHaveCSS('color', neutral);
    await expect(page.getByTestId('dashboard-projected')).toHaveCSS('color', neutral);
  });

  test('ShouldShowSpendingSkeletonUntilApiResponds', async ({ page }) => {
    await signedIn(page);
    let respond: () => void = () => {};
    await page.route(summaryRoute, async (route) => {
      await new Promise<void>((resolve) => (respond = resolve));
      await route.fulfill({ json: aSummary() });
    });

    await page.goto('/');

    await expect(page.getByTestId('dashboard-spending-skeleton')).toBeVisible();
    respond();
    await expect(page.getByTestId('dashboard-current-spending')).toHaveText('$1,847.63');
    await expect(page.getByTestId('dashboard-spending-skeleton')).toHaveCount(0);
  });

  test('ShouldShowErrorWhenSummaryFails', async ({ page }) => {
    await signedIn(page);
    await page.route(summaryRoute, (route) => route.fulfill({ status: 500, body: 'boom' }));

    await page.goto('/');

    await expect(page.getByTestId('dashboard-spending-error')).toHaveRole('alert');
    await expect(page.getByTestId('dashboard-spending-hero')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-spending-skeleton')).toHaveCount(0);
  });
});
