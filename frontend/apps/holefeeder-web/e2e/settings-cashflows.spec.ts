import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

type CashflowJson = {
  id: string;
  title: string;
  description: string;
  categoryName: string;
  categoryColor: string;
  categoryType: 'expense' | 'gain';
  accountName: string;
  amount: number;
  intervalType: 'daily' | 'weekly' | 'monthly' | 'yearly' | 'oneTime';
  frequency: number;
  effectiveDate: string;
  nextDate: string | null;
  tags: string[];
};

const listRoute = '**/gateway/api/v2/cashflows/manage';

const rent: CashflowJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3321',
  title: 'Rent',
  description: 'Rent',
  categoryName: 'Housing',
  categoryColor: '#5856D6',
  categoryType: 'expense',
  accountName: 'Chequing',
  amount: 1450,
  intervalType: 'monthly',
  frequency: 1,
  effectiveDate: '2025-09-01',
  nextDate: '2026-12-01',
  tags: ['home'],
};
const gym: CashflowJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3322',
  title: 'Health',
  description: '',
  categoryName: 'Health',
  categoryColor: '#30B0C7',
  categoryType: 'expense',
  accountName: 'Visa',
  amount: 45,
  intervalType: 'weekly',
  frequency: 2,
  effectiveDate: '2026-11-02',
  nextDate: '2026-11-16',
  tags: [],
};
const deposit: CashflowJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3323',
  title: 'Deposit',
  description: 'Deposit',
  categoryName: 'Housing',
  categoryColor: '#5856D6',
  categoryType: 'expense',
  accountName: 'Chequing',
  amount: 900,
  intervalType: 'oneTime',
  frequency: 1,
  effectiveDate: '2026-10-01',
  nextDate: null,
  tags: [],
};
const payroll: CashflowJson = {
  id: '3f2504e0-4f89-11d3-9a0c-0305e82c3324',
  title: 'Payroll',
  description: 'Payroll',
  categoryName: 'Salary',
  categoryColor: '#34C759',
  categoryType: 'gain',
  accountName: 'Chequing',
  amount: 2400,
  intervalType: 'weekly',
  frequency: 2,
  effectiveDate: '2026-01-02',
  nextDate: '2026-11-20',
  tags: [],
};

const withCashflows = async (page: Page, cashflows: CashflowJson[] = [gym, rent, deposit, payroll]) => {
  let calls = 0;
  await page.route(listRoute, (route) => {
    calls += 1;
    return route.fulfill({ json: cashflows });
  });
  return () => calls;
};

const rowOf = (page: Page, title: string) => page.getByTestId('cashflows-row').filter({ hasText: title });

const modifyRoute = '**/gateway/api/v2/cashflows/modify';

const capture = async (page: Page, url: string, status = 204) => {
  const bodies: unknown[] = [];
  await page.route(url, (route) => {
    bodies.push(route.request().postDataJSON());
    return route.fulfill({ status: status });
  });
  return bodies;
};

const withTagSuggestions = (page: Page) =>
  page.route('**/gateway/api/v2/transactions/tag-suggestions*', (route) => route.fulfill({ json: ['utilities'] }));

const openRent = async (page: Page) => {
  await withTagSuggestions(page);
  await page.goto('/settings/cashflows');
  await rowOf(page, 'Rent').getByTestId('cashflows-row-open').click();
  return page.getByTestId('cashflow');
};

test.describe('Settings cashflows', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldOpenFromTheSettings', async ({ page }) => {
    await withCashflows(page);
    await page.route('**/gateway/api/v2/settings', (route) =>
      route.fulfill({ json: { effectiveDate: '2026-09-01', intervalType: 'monthly', frequency: 1 } })
    );
    await page.goto('/settings');

    await page.getByTestId('settings-cashflows').click();

    await expect(page).toHaveURL(/\/settings\/cashflows$/);
    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Cashflows');
    await expect(page.getByTestId('nav-settings')).toHaveAttribute('aria-current', 'page');
  });

  test('ShouldGoBackToTheSettings', async ({ page }) => {
    await withCashflows(page);
    await page.goto('/settings/cashflows');

    await page.getByTestId('cashflows-back').click();

    await expect(page).toHaveURL(/\/settings$/);
  });

  test('ShouldListTheCashflowsBySection', async ({ page }) => {
    await withCashflows(page);

    await page.goto('/settings/cashflows');

    await expect(page.getByTestId('cashflows-expenses').getByTestId('cashflows-row')).toHaveText([/Health/, /Rent/, /Deposit/]);
    await expect(page.getByTestId('cashflows-income').getByTestId('cashflows-row')).toHaveText([/Payroll/]);
    const sections = page.locator('[data-testid="cashflows-expenses"], [data-testid="cashflows-income"]');
    expect(await sections.evaluateAll((elements) => elements.map((element) => element.getAttribute('data-testid')))).toEqual([
      'cashflows-expenses',
      'cashflows-income',
    ]);
  });

  test('ShouldShowWhatEachCashflowIs', async ({ page }) => {
    await withCashflows(page);

    await page.goto('/settings/cashflows');

    await expect(rowOf(page, 'Rent').getByTestId('cashflows-row-title')).toHaveText('Rent');
    await expect(rowOf(page, 'Rent').getByTestId('cashflows-row-details')).toHaveText('Housing · Chequing');
    await expect(rowOf(page, 'Health').getByTestId('cashflows-row-details')).toHaveText('Health · Visa');
  });

  test('ShouldShowTheAmountAndTheSchedule', async ({ page }) => {
    await withCashflows(page);

    await page.goto('/settings/cashflows');

    await expect(rowOf(page, 'Rent').getByTestId('cashflows-row-amount')).toHaveText('$1,450.00');
    await expect(rowOf(page, 'Rent').getByTestId('cashflows-row-schedule')).toHaveText('Monthly · next Dec 1, 2026');
    await expect(rowOf(page, 'Health').getByTestId('cashflows-row-schedule')).toHaveText('Every 2 week(s) · next Nov 16, 2026');
    await expect(rowOf(page, 'Payroll').getByTestId('cashflows-row-amount')).toHaveText('$2,400.00');
  });

  test('ShouldShowOnlyTheScheduleOfACashflowWithNothingLeftToPay', async ({ page }) => {
    await withCashflows(page);

    await page.goto('/settings/cashflows');

    await expect(rowOf(page, 'Deposit').getByTestId('cashflows-row-schedule')).toHaveText('One Time');
  });

  test('ShouldLeaveOutEmptySections', async ({ page }) => {
    await withCashflows(page, [rent]);

    await page.goto('/settings/cashflows');

    await expect(page.getByTestId('cashflows-expenses')).toBeVisible();
    await expect(page.getByTestId('cashflows-income')).toHaveCount(0);
  });

  test('ShouldExplainHowToAddACashflow_WhenThereAreNone', async ({ page }) => {
    await withCashflows(page, []);

    await page.goto('/settings/cashflows');

    await expect(page.getByTestId('cashflows-empty')).toContainText('No cashflows yet');
    await expect(page.getByTestId('cashflows-empty')).toContainText('Repeat a new transaction to schedule one.');
  });

  test('ShouldReportCashflowsThatFailToLoad', async ({ page }) => {
    await page.route(listRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto('/settings/cashflows');

    await expect(page.getByTestId('cashflows-load-error')).toHaveText('The cashflows could not be loaded.');
  });

  test('ShouldOpenTheCashflowFromItsRow', async ({ page }) => {
    await withCashflows(page);

    const flyout = await openRent(page);

    await expect(flyout.getByRole('heading', { level: 2 })).toHaveText('Edit cashflow');
    await expect(flyout.getByTestId('cashflow-summary')).toHaveText(/Housing · Chequing.*Monthly · next Dec 1, 2026/);
    await expect(flyout.getByTestId('cashflow-amount')).toHaveValue('1450.00');
    await expect(flyout.getByTestId('cashflow-date')).toHaveValue('2025-09-01');
    await expect(flyout.getByTestId('cashflow-description')).toHaveValue('Rent');
    await expect(flyout.getByTestId('cashflow-tag')).toHaveText(['home']);
  });

  test('ShouldSaveTheChanges', async ({ page }) => {
    const listCalls = await withCashflows(page);
    const bodies = await capture(page, modifyRoute);
    const flyout = await openRent(page);
    const callsBefore = listCalls();

    await flyout.getByTestId('cashflow-amount').fill('1500');
    await flyout.getByTestId('cashflow-date').fill('2026-01-01');
    await flyout.getByTestId('cashflow-description').fill('New rent');
    await flyout.getByTestId('cashflow-tag-input').fill('rent');
    await flyout.getByTestId('cashflow-tag-input').press('Enter');
    await flyout.getByTestId('cashflow-save').click();

    await expect(flyout).toBeHidden();
    expect(bodies).toEqual([{ id: rent.id, amount: '1500.00', effectiveDate: '2026-01-01', description: 'New rent', tags: ['home', 'rent'] }]);
    await expect(page.getByTestId('dashboard-toast')).toHaveText('Cashflow saved');
    await expect.poll(listCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldOnlySaveAValidAmountAndDate', async ({ page }) => {
    await withCashflows(page);
    const flyout = await openRent(page);

    await flyout.getByTestId('cashflow-amount').fill('12.345');
    await expect(flyout.getByTestId('cashflow-save')).toBeDisabled();
    await flyout.getByTestId('cashflow-amount').fill('12.34');
    await expect(flyout.getByTestId('cashflow-save')).toBeEnabled();
    await flyout.getByTestId('cashflow-date').fill('');
    await expect(flyout.getByTestId('cashflow-save')).toBeDisabled();
  });

  test('ShouldKeepTheFlyoutOpenWhenSavingFails', async ({ page }) => {
    await withCashflows(page);
    await capture(page, modifyRoute, 500);
    const flyout = await openRent(page);

    await flyout.getByTestId('cashflow-save').click();

    await expect(flyout.getByTestId('cashflow-error')).toHaveText('The cashflow could not be saved. Try again.');
    await expect(flyout).toBeVisible();
  });

  test('ShouldCloseWithoutSaving', async ({ page }) => {
    await withCashflows(page);
    const bodies = await capture(page, modifyRoute);
    const flyout = await openRent(page);

    await flyout.getByTestId('cashflow-description').fill('Changed');
    await flyout.getByTestId('cashflow-cancel').click();

    await expect(flyout).toBeHidden();
    expect(bodies).toEqual([]);
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withCashflows(page);

      await page.goto('/settings/cashflows');

      await expect(rowOf(page, 'Health')).toBeVisible();
      const schedule = await rowOf(page, 'Health').getByTestId('cashflows-row-schedule').boundingBox();
      expect(schedule!.x + schedule!.width).toBeLessThanOrEqual(360);
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });

    test('ShouldOpenTheFlyoutFullWidth', async ({ page }) => {
      await withCashflows(page);

      const flyout = await openRent(page);

      await expect(flyout.getByTestId('cashflow-amount')).toBeVisible();
      await expect.poll(async () => Math.round((await flyout.boundingBox())?.width ?? 0)).toBe(360);
      const overflow = await flyout.evaluate((element) => element.scrollWidth - element.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
