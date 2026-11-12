import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const incomeRoute = '**/gateway/api/v2/insights/income';

const positive = 'rgb(46, 125, 50)';
const negative = 'rgb(211, 47, 47)';

const salary = {
  id: 'c1',
  name: 'Salary',
  color: '#34C759',
  budget: 0,
  budgetStatus: 'none',
  budgetPercentage: 0,
  amount: 3500,
  average: 3000,
  tags: [{ tag: 'job', amount: 3500 }],
};
const interest = {
  id: 'c2',
  name: 'Interest',
  color: '#5856D6',
  budget: 0,
  budgetStatus: 'none',
  budgetPercentage: 0,
  amount: 12.5,
  average: 0,
  tags: [],
};

const aBreakdown = (overrides: Record<string, unknown> = {}) => ({
  period: { start: '2026-11-01', end: '2026-11-30' },
  current: 3512.5,
  average: 3262.5,
  variation: 250,
  variationPercentage: 7.66,
  categories: [salary, interest],
  tags: [{ tag: 'job', amount: 3500, average: 3000 }],
  ...overrides,
});

const withIncome = (page: Page, breakdown: unknown = aBreakdown()) => page.route(incomeRoute, (route) => route.fulfill({ json: breakdown }));

const categoryRow = (page: Page, name: string) =>
  page.getByTestId('income-category').filter({ has: page.getByTestId('income-category-name').getByText(name, { exact: true }) });

test.describe('Insights income', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldShowTheIncomePage', async ({ page }) => {
    await withIncome(page);

    await page.goto('/insights/income');

    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Income');
    await expect(page.getByTestId('income-combined')).toHaveCount(0);
  });

  test('ShouldGoBack', async ({ page }) => {
    await withIncome(page);
    await page.goto('/insights/income');

    await page.getByTestId('income-back').click();

    await expect(page).toHaveURL(/\/insights$/);
  });

  test('ShouldShowAGoodTrend_WhenIncomeIsAboveAverage', async ({ page }) => {
    await withIncome(page);

    await page.goto('/insights/income');

    await expect(page.getByTestId('income-hero').getByText('Current Income')).toBeVisible();
    await expect(page.getByTestId('income-current')).toHaveText('$3,512.50');
    await expect(page.getByTestId('income-average')).toHaveText('$3,262.50');
    await expect(page.getByTestId('income-trend')).toHaveText('↗$250.00 vs Average');
    await expect(page.getByTestId('income-trend')).toHaveCSS('color', positive);
  });

  test('ShouldFlagTheTrend_WhenIncomeIsBelowAverage', async ({ page }) => {
    await withIncome(page, aBreakdown({ current: 3000, variation: -262.5 }));

    await page.goto('/insights/income');

    await expect(page.getByTestId('income-trend')).toHaveText('↘$262.50 vs Average');
    await expect(page.getByTestId('income-trend')).toHaveCSS('color', negative);
  });

  test('ShouldListTheIncomeByCategory', async ({ page }) => {
    await withIncome(page);

    await page.goto('/insights/income');

    const section = page.getByTestId('income-categories');
    await expect(section.getByText('Income by Category')).toBeVisible();
    await expect(section.getByTestId('income-category-name')).toHaveText(['Salary', 'Interest']);
    const row = categoryRow(page, 'Salary');
    await expect(row.getByTestId('income-category-amount')).toHaveText('$3,500.00');
    await expect(row.getByTestId('income-category-average')).toHaveText('avg $3,000.00');
    await expect(row.getByTestId('income-category-budget')).toHaveCount(0);
    await expect(categoryRow(page, 'Interest').getByTestId('income-category-average')).toHaveCount(0);
  });

  test('ShouldListTheIncomeByTag', async ({ page }) => {
    await withIncome(page);

    await page.goto('/insights/income');

    const section = page.getByTestId('income-tags');
    await expect(section.getByText('Income by Tag')).toBeVisible();
    await expect(section.getByTestId('income-tag-name')).toHaveText(['job']);
    await expect(section.getByTestId('income-tag-amount')).toHaveText(['$3,500.00']);
  });

  test('ShouldSayWhenThereIsNothingToShow', async ({ page }) => {
    await withIncome(page, aBreakdown({ categories: [], tags: [] }));

    await page.goto('/insights/income');

    await expect(page.getByTestId('income-categories').getByText('No income this period')).toBeVisible();
    await expect(page.getByTestId('income-tags').getByText('No tagged income this period')).toBeVisible();
  });

  test('ShouldShowLoading', async ({ page }) => {
    await page.route(incomeRoute, () => undefined);

    await page.goto('/insights/income');

    await expect(page.getByTestId('income-loading')).toBeVisible();
  });

  test('ShouldReportAFailureToLoad', async ({ page }) => {
    await page.route(incomeRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto('/insights/income');

    await expect(page.getByTestId('income-load-error')).toHaveText('Insights could not be loaded.');
  });

  test.describe('in French', () => {
    test.use({ locale: 'fr-CA' });

    test('ShouldShowTheIncomeInFrench', async ({ page }) => {
      await withIncome(page);

      await page.goto('/insights/income');

      await expect(page.getByRole('heading', { level: 1 })).toHaveText('Revenus');
      await expect(page.getByTestId('income-categories').getByText('Revenus par catégorie')).toBeVisible();
    });
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withIncome(
        page,
        aBreakdown({ categories: [{ ...salary, name: 'A very long category name that keeps going and going and going' }] })
      );

      await page.goto('/insights/income');

      await expect(page.getByTestId('income-category')).toBeVisible();
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
