import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const overviewRoute = '**/gateway/api/v2/insights/overview*';

const positive = 'rgb(46, 125, 50)';
const negative = 'rgb(211, 47, 47)';

const month = (start: string, end: string, income: number, spending: number) => ({
  period: { start: start, end: end },
  income: income,
  spending: spending,
  net: Math.round((income - spending) * 100) / 100,
});

const anOverview = (overrides: Record<string, unknown> = {}) => ({
  period: { start: '2026-11-01', end: '2026-11-30' },
  income: 4800,
  spending: 3559.63,
  net: 1240.37,
  outcome: 'saved',
  savedPercentage: 26,
  series: [
    month('2026-06-01', '2026-06-30', 4800, 3600),
    month('2026-07-01', '2026-07-31', 4800, 4100),
    month('2026-08-01', '2026-08-31', 5200, 3900),
    month('2026-09-01', '2026-09-30', 4800, 5300),
    month('2026-10-01', '2026-10-31', 3900, 4400),
    month('2026-11-01', '2026-11-30', 4800, 3559.63),
  ],
  totalIncome: 28300,
  totalSpending: 24859.63,
  totalNet: 3440.37,
  range: { unit: 'month', amount: 6 },
  ...overrides,
});

const pointAt = async (page: Page, index: number, count = 6) => {
  const box = (await page.getByTestId('insights-chart-hit').boundingBox())!;
  await page.mouse.move(box.x + (box.width * index) / (count - 1), box.y + box.height / 2);
};

const withOverview = (page: Page, overview: unknown = anOverview()) => page.route(overviewRoute, (route) => route.fulfill({ json: overview }));

test.describe('Insights', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldOpenFromTheNavigation', async ({ page }) => {
    await withOverview(page);
    await page.goto('/');

    await page.getByRole('navigation').getByTestId('nav-insights').click();

    await expect(page).toHaveURL(/\/insights$/);
    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Insights');
    await expect(page.getByTestId('nav-insights')).toHaveAttribute('aria-current', 'page');
  });

  test('ShouldShowWhatWasSavedThisPeriod', async ({ page }) => {
    await withOverview(page);

    await page.goto('/insights');

    await expect(page.getByTestId('insights-hero').getByText('Net this period')).toBeVisible();
    await expect(page.getByTestId('insights-net')).toHaveText('$1,240.37');
    await expect(page.getByTestId('insights-outcome')).toHaveText('Saved 26% of income');
    await expect(page.getByTestId('insights-outcome')).toHaveCSS('color', positive);
    await expect(page.getByTestId('insights-income')).toHaveText('$4,800.00');
    await expect(page.getByTestId('insights-spending')).toHaveText('$3,559.63');
  });

  test('ShouldShowWhatWasOverspentThisPeriod', async ({ page }) => {
    await withOverview(page, anOverview({ income: 0, spending: 50, net: -50, outcome: 'overspent', savedPercentage: 0 }));

    await page.goto('/insights');

    await expect(page.getByTestId('insights-net')).toHaveText('$50.00');
    await expect(page.getByTestId('insights-outcome')).toHaveText('Overspent by $50.00');
    await expect(page.getByTestId('insights-outcome')).toHaveCSS('color', negative);
  });

  test('ShouldShowBreakingEven', async ({ page }) => {
    await withOverview(page, anOverview({ income: 80, spending: 80, net: 0, outcome: 'brokeEven', savedPercentage: 0 }));

    await page.goto('/insights');

    await expect(page.getByTestId('insights-outcome')).toHaveText('Broke even');
  });

  test('ShouldShowNoOutcome_WhenThePeriodHasNoActivity', async ({ page }) => {
    await withOverview(page, anOverview({ income: 0, spending: 0, net: 0, outcome: 'none', savedPercentage: 0 }));

    await page.goto('/insights');

    await expect(page.getByTestId('insights-net')).toHaveText('$0.00');
    await expect(page.getByTestId('insights-outcome')).toHaveCount(0);
  });

  test('ShouldOpenTheIncomeThisPeriod', async ({ page }) => {
    await withOverview(page);
    await page.goto('/insights');

    const link = page.getByTestId('insights-income-link');
    await expect(link).toContainText('Income this period');
    await expect(link).toContainText('$4,800.00');
    await link.click();

    await expect(page).toHaveURL(/\/insights\/income$/);
    await expect(page.getByTestId('nav-insights')).toHaveAttribute('aria-current', 'page');
  });

  test('ShouldOpenTheSpendingThisPeriod', async ({ page }) => {
    await withOverview(page);
    await page.goto('/insights');

    const link = page.getByTestId('insights-spending-link');
    await expect(link).toContainText('Spending this period');
    await expect(link).toContainText('$3,559.63');
    await link.click();

    await expect(page).toHaveURL(/\/insights\/spending$/);
  });

  test('ShouldDrawIncomeAndExpensesOverThePeriods', async ({ page }) => {
    await withOverview(page);

    await page.goto('/insights');

    const chart = page.getByTestId('insights-chart');
    await expect(page.getByTestId('insights-chart-section').getByText('Income vs Expenses')).toBeVisible();
    await expect(chart.getByTestId('insights-chart-legend')).toHaveText(['Income', 'Expenses', 'Deficit']);
    await expect(chart.getByTestId('insights-chart-income')).toHaveAttribute('d', /^M[\d.]+,[\d.]+(L[\d.]+,[\d.]+){5}$/);
    await expect(chart.getByTestId('insights-chart-expenses')).toHaveAttribute('stroke-dasharray', '6 4');
    await expect(chart.getByTestId('insights-chart-x-label')).toHaveText(['Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov']);
    await expect(chart.getByTestId('insights-chart-y-label')).toHaveText(['$0', '$2K', '$4K', '$6K']);
  });

  test('ShouldShadeTheSurplusAndTheDeficit', async ({ page }) => {
    await withOverview(page);

    await page.goto('/insights');

    const bands = page.getByTestId('insights-chart-band');
    await expect(bands).toHaveCount(3);
    expect(await bands.evaluateAll((elements) => elements.map((element) => element.getAttribute('data-kind')))).toEqual([
      'surplus',
      'deficit',
      'surplus',
    ]);
  });

  test('ShouldDrawFlatLines_WhenNothingHappened', async ({ page }) => {
    await withOverview(
      page,
      anOverview({
        series: [month('2026-10-01', '2026-10-31', 0, 0), month('2026-11-01', '2026-11-30', 0, 0)],
        totalIncome: 0,
        totalSpending: 0,
        totalNet: 0,
      })
    );

    await page.goto('/insights');

    await expect(page.getByTestId('insights-chart-income')).toHaveAttribute('d', /^M[\d.]+,[\d.]+L[\d.]+,[\d.]+$/);
    await expect(page.getByTestId('insights-chart-band')).toHaveCount(0);
  });

  test('ShouldTotalTheRange', async ({ page }) => {
    await withOverview(page);

    await page.goto('/insights');

    await expect(page.getByTestId('insights-total-income')).toHaveText('$28,300.00');
    await expect(page.getByTestId('insights-total-expenses')).toHaveText('$24,859.63');
    await expect(page.getByTestId('insights-total-net')).toHaveText('$3,440.37');
    await expect(page.getByTestId('insights-total-net')).toHaveCSS('color', positive);
    await expect(page.getByTestId('insights-chart-footnote')).toHaveText('Excludes transfers between your accounts');
  });

  test('ShouldShowANegativeNetOverTheRange', async ({ page }) => {
    await withOverview(page, anOverview({ totalNet: -120.5 }));

    await page.goto('/insights');

    await expect(page.getByTestId('insights-total-net')).toHaveText('-$120.50');
    await expect(page.getByTestId('insights-total-net')).toHaveCSS('color', negative);
  });

  test('ShouldOfferTheChartAsATable', async ({ page }) => {
    await withOverview(page);

    await page.goto('/insights');

    const rows = page.getByTestId('insights-chart-table').locator('tbody tr');
    await expect(rows).toHaveCount(6);
    await expect(rows.first()).toHaveText(/Jun.*\$4,800\.00.*\$3,600\.00/);
  });

  test('ShouldShowThePeriodUnderThePointer', async ({ page }) => {
    await withOverview(page);
    await page.goto('/insights');

    await pointAt(page, 3);

    const tooltip = page.getByTestId('insights-chart-tooltip');
    await expect(tooltip.getByTestId('insights-chart-tooltip-title')).toHaveText('September 2026');
    await expect(tooltip.getByTestId('insights-chart-tooltip-income')).toHaveText(/Income.*\$4,800\.00/);
    await expect(tooltip.getByTestId('insights-chart-tooltip-expenses')).toHaveText(/Expenses.*\$5,300\.00/);
    await expect(tooltip.getByTestId('insights-chart-tooltip-net')).toHaveText('Overspent by $500.00');
    await expect(tooltip.getByTestId('insights-chart-tooltip-net')).toHaveCSS('color', negative);
    await expect(page.getByTestId('insights-chart-marker')).toBeVisible();
  });

  test('ShouldShowWhatWasSaved_WhenPointingAtASurplus', async ({ page }) => {
    await withOverview(page);
    await page.goto('/insights');

    await pointAt(page, 0);

    await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('June 2026');
    await expect(page.getByTestId('insights-chart-tooltip-net')).toHaveText('Saved $1,200.00');
    await expect(page.getByTestId('insights-chart-tooltip-net')).toHaveCSS('color', positive);
  });

  test('ShouldHideTheTooltip_WhenThePointerLeaves', async ({ page }) => {
    await withOverview(page);
    await page.goto('/insights');
    await pointAt(page, 2);

    await page.getByTestId('insights-hero').hover();

    await expect(page.getByTestId('insights-chart-tooltip')).toHaveCount(0);
    await expect(page.getByTestId('insights-chart-marker')).toHaveCount(0);
  });

  test('ShouldMoveBetweenPeriodsWithTheArrowKeys', async ({ page }) => {
    await withOverview(page);
    await page.goto('/insights');

    await page.getByTestId('insights-chart-plot').focus();
    await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('November 2026');

    await page.keyboard.press('ArrowLeft');
    await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('October 2026');

    await page.keyboard.press('Home');
    await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('June 2026');

    await page.keyboard.press('ArrowLeft');
    await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('June 2026');

    await page.keyboard.press('ArrowRight');
    await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('July 2026');

    await page.keyboard.press('End');
    await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('November 2026');

    await page.keyboard.press('Escape');
    await expect(page.getByTestId('insights-chart-tooltip')).toHaveCount(0);
  });

  test('ShouldShowLoading', async ({ page }) => {
    await page.route(overviewRoute, () => undefined);

    await page.goto('/insights');

    await expect(page.getByTestId('insights-loading')).toBeVisible();
  });

  test('ShouldReportAFailureToLoad', async ({ page }) => {
    await page.route(overviewRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto('/insights');

    await expect(page.getByTestId('insights-load-error')).toHaveText('Insights could not be loaded.');
  });

  test.describe('in French', () => {
    test.use({ locale: 'fr-CA' });

    test('ShouldShowTheInsightsInFrench', async ({ page }) => {
      await withOverview(page);

      await page.goto('/insights');

      await expect(page.getByRole('heading', { level: 1 })).toHaveText('Aperçus');
      await expect(page.getByTestId('insights-outcome')).toHaveText('Épargné 26 % du revenu');
    });
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 }, hasTouch: true });

    test('ShouldShowThePeriodTapped', async ({ page }) => {
      await withOverview(page);
      await page.goto('/insights');

      const box = (await page.getByTestId('insights-chart-hit').boundingBox())!;
      await page.touchscreen.tap(box.x + box.width / 5, box.y + box.height / 2);

      await expect(page.getByTestId('insights-chart-tooltip-title')).toHaveText('July 2026');
    });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withOverview(page, anOverview({ net: 1234567.89, income: 9876543.21, spending: 8641975.32 }));

      await page.goto('/insights');

      await expect(page.getByTestId('insights-net')).toBeVisible();
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
