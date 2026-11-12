import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const spendingRoute = '**/gateway/api/v2/insights/spending';

const positive = 'rgb(46, 125, 50)';
const negative = 'rgb(211, 47, 47)';

type TagAmountJson = { tag: string; amount: number };
type CategoryJson = {
  id: string;
  name: string;
  color: string;
  budget: number;
  budgetStatus: 'none' | 'under' | 'over';
  budgetPercentage: number;
  amount: number;
  average: number;
  tags: TagAmountJson[];
};
type TagJson = { tag: string; amount: number; average: number };

const groceries: CategoryJson = {
  id: 'c1',
  name: 'Groceries',
  color: '#34C759',
  budget: 500,
  budgetStatus: 'under',
  budgetPercentage: 97,
  amount: 485.12,
  average: 412.3,
  tags: [
    { tag: 'costco', amount: 300 },
    { tag: 'weekly', amount: 185.12 },
  ],
};
const dining: CategoryJson = {
  id: 'c2',
  name: 'Dining',
  color: '#FF2D55',
  budget: 100,
  budgetStatus: 'over',
  budgetPercentage: 160,
  amount: 160,
  average: 90,
  tags: [],
};
const gym: CategoryJson = {
  id: 'c3',
  name: 'Gym',
  color: '#5856D6',
  budget: 0,
  budgetStatus: 'none',
  budgetPercentage: 0,
  amount: 40,
  average: 40,
  tags: [{ tag: 'health', amount: 40 }],
};

const costco: TagJson = { tag: 'costco', amount: 300, average: 250.5 };
const weekly: TagJson = { tag: 'weekly', amount: 185.12, average: 0 };

const aBreakdown = (overrides: Record<string, unknown> = {}) => ({
  period: { start: '2026-11-01', end: '2026-11-30' },
  current: 685.12,
  average: 297.52,
  variation: 387.6,
  variationPercentage: 130.28,
  categories: [groceries, dining, gym],
  tags: [costco, weekly],
  ...overrides,
});

const withSpending = async (page: Page, breakdown: unknown = aBreakdown()) => {
  let authorization: string | undefined;
  await page.route(spendingRoute, (route) => {
    authorization = route.request().headers()['authorization'];
    return route.fulfill({ json: breakdown });
  });
  return () => authorization;
};

const categoryRow = (page: Page, name: string) =>
  page.getByTestId('spending-category').filter({ has: page.getByTestId('spending-category-name').getByText(name, { exact: true }) });

const tagRow = (page: Page, tag: string) =>
  page.getByTestId('spending-tag').filter({ has: page.getByTestId('spending-tag-name').getByText(tag, { exact: true }) });

test.describe('Insights spending', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldShowTheSpendingPage', async ({ page }) => {
    const authorization = await withSpending(page);

    await page.goto('/insights/spending');

    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Spending');
    expect(authorization()).toBe('Bearer e2e-token');
  });

  test('ShouldGoBack', async ({ page }) => {
    await withSpending(page);
    await page.goto('/insights/spending');

    await page.getByTestId('spending-back').click();

    await expect(page).not.toHaveURL(/\/insights\/spending$/);
  });

  test('ShouldCompareTheSpendingWithItsAverage', async ({ page }) => {
    await withSpending(page);

    await page.goto('/insights/spending');

    const hero = page.getByTestId('spending-hero');
    await expect(hero.getByText('Current Spending')).toBeVisible();
    await expect(page.getByTestId('spending-current')).toHaveText('$685.12');
    await expect(page.getByTestId('spending-average')).toHaveText('$297.52');
    await expect(page.getByTestId('spending-trend')).toHaveText('↗$387.60 vs Average');
    await expect(page.getByTestId('spending-trend')).toHaveCSS('color', negative);
  });

  test('ShouldShowAGoodTrend_WhenSpendingIsBelowAverage', async ({ page }) => {
    await withSpending(page, aBreakdown({ current: 200, variation: -97.52 }));

    await page.goto('/insights/spending');

    await expect(page.getByTestId('spending-trend')).toHaveText('↘$97.52 vs Average');
    await expect(page.getByTestId('spending-trend')).toHaveCSS('color', positive);
  });

  test('ShouldHideTheTrend_WhenThereIsNoAverage', async ({ page }) => {
    await withSpending(page, aBreakdown({ average: 0, variation: 685.12 }));

    await page.goto('/insights/spending');

    await expect(page.getByTestId('spending-current')).toHaveText('$685.12');
    await expect(page.getByTestId('spending-trend')).toHaveCount(0);
  });

  test('ShouldListTheCategoriesInTheOrderReceived', async ({ page }) => {
    await withSpending(page);

    await page.goto('/insights/spending');

    const section = page.getByTestId('spending-categories');
    await expect(section.getByText('Spending by Category')).toBeVisible();
    await expect(section.getByTestId('spending-category-name')).toHaveText(['Groceries', 'Dining', 'Gym']);
  });

  test('ShouldShowHowMuchOfItsBudgetACategoryUsed', async ({ page }) => {
    await withSpending(page);

    await page.goto('/insights/spending');

    const row = categoryRow(page, 'Groceries');
    await expect(row.getByTestId('spending-category-amount')).toHaveText('$485.12');
    await expect(row.getByTestId('spending-category-amount')).toHaveCSS('color', positive);
    await expect(row.getByTestId('spending-category-budget')).toHaveText('97%');
    await expect(row.getByTestId('spending-category-average')).toHaveText('avg $412.30');
  });

  test('ShouldFlagACategoryOverItsBudget', async ({ page }) => {
    await withSpending(page);

    await page.goto('/insights/spending');

    const row = categoryRow(page, 'Dining');
    await expect(row.getByTestId('spending-category-amount')).toHaveCSS('color', negative);
    await expect(row.getByTestId('spending-category-budget')).toHaveText('Over budget');
  });

  test('ShouldShowNoBudget_WhenTheCategoryHasNone', async ({ page }) => {
    await withSpending(page);

    await page.goto('/insights/spending');

    await expect(categoryRow(page, 'Gym').getByTestId('spending-category-budget')).toHaveCount(0);
  });

  test('ShouldListTheTagsInTheOrderReceived', async ({ page }) => {
    await withSpending(page);

    await page.goto('/insights/spending');

    const section = page.getByTestId('spending-tags');
    await expect(section.getByText('Spending by Tag')).toBeVisible();
    await expect(section.getByTestId('spending-tag-name')).toHaveText(['costco', 'weekly']);
    await expect(tagRow(page, 'costco').getByTestId('spending-tag-amount')).toHaveText('$300.00');
    await expect(tagRow(page, 'costco').getByTestId('spending-tag-average')).toHaveText('avg $250.50');
    await expect(tagRow(page, 'weekly').getByTestId('spending-tag-average')).toHaveCount(0);
  });

  test('ShouldCombineCategoriesAndTheirTags', async ({ page }) => {
    await withSpending(page);
    await page.goto('/insights/spending');

    await page.getByTestId('spending-combined').click();

    const section = page.getByTestId('spending-combined-breakdown');
    await expect(section.getByText('Spending by Category & Tag')).toBeVisible();
    await expect(page.getByTestId('spending-categories')).toHaveCount(0);
    await expect(page.getByTestId('spending-tags')).toHaveCount(0);
    await expect(section.getByTestId('spending-combined-row')).toHaveText([
      /Groceries.*\$485\.12/,
      /costco.*\$300\.00/,
      /weekly.*\$185\.12/,
      /Dining.*\$160\.00/,
      /Gym.*\$40\.00/,
      /health.*\$40\.00/,
    ]);
  });

  test('ShouldSayWhenThereIsNothingToShow', async ({ page }) => {
    await withSpending(page, aBreakdown({ categories: [], tags: [] }));

    await page.goto('/insights/spending');

    await expect(page.getByTestId('spending-categories').getByText('No expenses this period')).toBeVisible();
    await expect(page.getByTestId('spending-tags').getByText('No tagged expenses this period')).toBeVisible();
  });

  test('ShouldShowLoading', async ({ page }) => {
    await page.route(spendingRoute, () => undefined);

    await page.goto('/insights/spending');

    await expect(page.getByTestId('spending-loading')).toBeVisible();
  });

  test('ShouldReportAFailureToLoad', async ({ page }) => {
    await page.route(spendingRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto('/insights/spending');

    await expect(page.getByTestId('spending-load-error')).toHaveText('Insights could not be loaded.');
  });

  test.describe('in French', () => {
    test.use({ locale: 'fr-CA' });

    test('ShouldShowTheSpendingInFrench', async ({ page }) => {
      await withSpending(page);

      await page.goto('/insights/spending');

      await expect(page.getByRole('heading', { level: 1 })).toHaveText('Dépenses');
      await expect(categoryRow(page, 'Groceries').getByTestId('spending-category-budget')).toHaveText('97 %');
      await expect(categoryRow(page, 'Dining').getByTestId('spending-category-budget')).toHaveText('Budget dépassé');
    });
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withSpending(
        page,
        aBreakdown({ categories: [{ ...groceries, name: 'A very long category name that keeps going and going and going' }] })
      );

      await page.goto('/insights/spending');

      await expect(page.getByTestId('spending-category')).toBeVisible();
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
