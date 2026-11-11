import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const chequing = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3301', name: 'Chequing' };
const savings = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3302', name: 'Savings' };
const closed = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3303', name: 'Old savings' };
const groceries = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3311', name: 'Groceries', color: '#34C759', type: 'expense' };
const dining = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3312', name: 'Dining', color: '#FF9500', type: 'expense' };
const salary = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3313', name: 'Salary', color: '#007AFF', type: 'gain' };
const transferIn = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3314', name: 'Transfer In', color: '#AEAEB2', type: 'gain' };

const transactionId = '3f2504e0-4f89-11d3-9a0c-0305e82c3321';
const detailRoute = `**/gateway/api/v2/transactions/${transactionId}`;
const modifyRoute = '**/gateway/api/v2/transactions/modify';
const recentRoute = '**/gateway/api/v2/dashboard/transactions';
const accountTransactionsRoute = `**/gateway/api/v2/accounts/${chequing.id}/transactions**`;

type DetailJson = {
  id: string;
  date: string;
  amount: number;
  description: string;
  tags: string[];
  category: { id: string; name: string; type: string; color: string };
  account: { id: string; name: string };
};

const market: DetailJson = {
  id: transactionId,
  date: '2026-08-28',
  amount: 123.45,
  description: 'Market',
  tags: ['weekly', 'family'],
  category: groceries,
  account: chequing,
};

const row = {
  id: transactionId,
  title: 'Market',
  category: 'Groceries',
  categoryColor: '#34C759',
  tags: ['weekly'],
  amount: -123.45,
  date: '2026-08-28',
};

const withOptions = async (page: Page) => {
  await page.route('**/gateway/api/v2/transactions/new-transaction-options', (route) =>
    route.fulfill({ json: { accounts: [chequing, savings], categories: [groceries, dining, salary] } })
  );
  await page.route('**/gateway/api/v2/transactions/tag-suggestions**', (route) => route.fulfill({ json: ['costco'] }));
};

const withDetail = (page: Page, detail: DetailJson = market) => page.route(detailRoute, (route) => route.fulfill({ json: detail }));

const withRecent = async (page: Page) => {
  let calls = 0;
  await page.route(recentRoute, (route) => {
    calls += 1;
    return route.fulfill({ json: [row] });
  });
  return () => calls;
};

const capture = async (page: Page, status: number) => {
  const bodies: unknown[] = [];
  await page.route(modifyRoute, (route) => {
    bodies.push(route.request().postDataJSON());
    return route.fulfill({ status: status });
  });
  return bodies;
};

const openFromDashboard = async (page: Page) => {
  await page.goto('/');
  await page.getByTestId('dashboard-transaction-row').click();
  return page.getByTestId('edit-transaction');
};

const choose = async (page: Page, field: string, option: string) => {
  await page.getByTestId(field).click();
  await page.getByRole('option', { name: option }).click();
};

test.describe('Edit transaction', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    await withOptions(page);
  });

  test('ShouldOpenTheTransactionFromTheDashboard', async ({ page }) => {
    await withRecent(page);
    await withDetail(page);

    const flyout = await openFromDashboard(page);

    await expect(flyout.getByRole('heading')).toHaveText('Edit transaction');
    await expect(flyout.getByRole('button', { name: 'Expense' })).toHaveAttribute('aria-pressed', 'true');
    await expect(flyout.getByRole('button', { name: 'Transfer' })).toHaveCount(0);
    await expect(flyout.getByTestId('edit-transaction-amount')).toHaveValue('123.45');
    await expect(flyout.getByTestId('edit-transaction-date')).toHaveValue('2026-08-28');
    await expect(flyout.getByTestId('edit-transaction-account')).toHaveText('Chequing');
    await expect(flyout.getByTestId('edit-transaction-category')).toHaveText('Groceries');
    await expect(flyout.getByTestId('edit-transaction-tag')).toHaveText(['weekly', 'family']);
    await expect(flyout.getByTestId('edit-transaction-description')).toHaveValue('Market');
    await expect(flyout.getByText('Repeat')).toHaveCount(0);
  });

  test('ShouldOpenTheTransactionFromTheAccountPage', async ({ page }) => {
    await page.route(`**/gateway/api/v2/accounts/${chequing.id}/detail`, (route) =>
      route.fulfill({
        json: {
          id: chequing.id,
          name: 'Chequing',
          type: 'Checking',
          description: '',
          openBalance: 0,
          openDate: '2021-01-04',
          favorite: false,
          inactive: false,
          caption: 'availableBalance',
          balance: 100,
          upcoming: 0,
          projectedBalance: 100,
          updated: '2026-08-28',
        },
      })
    );
    await page.route(accountTransactionsRoute, (route) => route.fulfill({ json: [row], headers: { 'X-Total-Count': '1' } }));
    await withDetail(page);
    await page.goto(`/accounts/${chequing.id}`);

    await page.getByTestId('account-transaction-row').click();

    const flyout = page.getByTestId('edit-transaction');
    await expect(flyout.getByTestId('edit-transaction-amount')).toHaveValue('123.45');
    await expect(flyout.getByTestId('edit-transaction-account')).toHaveText('Chequing');
  });

  test('ShouldSaveTheChanges', async ({ page }) => {
    const recentCalls = await withRecent(page);
    await withDetail(page);
    const bodies = await capture(page, 204);
    const flyout = await openFromDashboard(page);
    const callsBefore = recentCalls();

    await flyout.getByTestId('edit-transaction-amount').fill('50.25');
    await flyout.getByTestId('edit-transaction-date').fill('2026-09-01');
    await choose(page, 'edit-transaction-account', 'Savings');
    await choose(page, 'edit-transaction-category', 'Dining');
    await flyout.getByRole('button', { name: 'Remove tag family' }).click();
    await flyout.getByTestId('edit-transaction-description').fill('Bistro');
    await flyout.getByTestId('edit-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transaction saved');
    await expect(page.getByTestId('edit-transaction')).toHaveCount(0);
    expect(bodies).toEqual([
      {
        id: transactionId,
        date: '2026-09-01',
        amount: '50.25',
        description: 'Bistro',
        accountId: savings.id,
        categoryId: dining.id,
        tags: ['weekly'],
      },
    ]);
    await expect.poll(recentCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldSwitchToTheCategoriesOfTheChosenType', async ({ page }) => {
    await withRecent(page);
    await withDetail(page);
    const flyout = await openFromDashboard(page);

    await flyout.getByRole('button', { name: 'Income' }).click();

    await expect(flyout.getByTestId('edit-transaction-category')).toHaveText('Salary');
    await flyout.getByTestId('edit-transaction-category').click();
    await expect(page.getByRole('option')).toHaveText(['Salary']);
  });

  test('ShouldKeepTheTransferCategoryAndClosedAccountOfOneSide', async ({ page }) => {
    await withRecent(page);
    await withDetail(page, { ...market, description: 'From chequing', category: transferIn, account: closed });
    const bodies = await capture(page, 204);
    const flyout = await openFromDashboard(page);

    await expect(flyout.getByRole('button', { name: 'Income' })).toHaveAttribute('aria-pressed', 'true');
    await expect(flyout.getByTestId('edit-transaction-category')).toHaveText('Transfer In');
    await expect(flyout.getByTestId('edit-transaction-account')).toHaveText('Old savings');
    await flyout.getByTestId('edit-transaction-category').click();
    await expect(page.getByRole('option')).toHaveText(['Salary', 'Transfer In']);
    await page.keyboard.press('Escape');
    await flyout.getByTestId('edit-transaction-amount').fill('100');
    await flyout.getByTestId('edit-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transaction saved');
    expect(bodies).toEqual([
      {
        id: transactionId,
        date: '2026-08-28',
        amount: '100.00',
        description: 'From chequing',
        accountId: closed.id,
        categoryId: transferIn.id,
        tags: ['weekly', 'family'],
      },
    ]);
  });

  test('ShouldOnlySaveAValidAmount', async ({ page }) => {
    await withRecent(page);
    await withDetail(page);
    const flyout = await openFromDashboard(page);

    await flyout.getByTestId('edit-transaction-amount').fill('12.345');

    await expect(flyout.getByTestId('edit-transaction-save')).toBeDisabled();
  });

  test('ShouldKeepTheFlyoutOpenWhenSavingFails', async ({ page }) => {
    await withRecent(page);
    await withDetail(page);
    await capture(page, 500);
    const flyout = await openFromDashboard(page);

    await flyout.getByTestId('edit-transaction-save').click();

    await expect(flyout.getByTestId('edit-transaction-error')).toHaveText('The transaction could not be saved. Try again.');
    await expect(flyout).toBeVisible();
  });

  test('ShouldCloseWithoutSaving', async ({ page }) => {
    await withRecent(page);
    await withDetail(page);
    const bodies = await capture(page, 204);
    const flyout = await openFromDashboard(page);

    await flyout.getByTestId('edit-transaction-cancel').click();

    await expect(page.getByTestId('edit-transaction')).toHaveCount(0);
    expect(bodies).toEqual([]);
  });

  test('ShouldReportATransactionThatFailsToLoad', async ({ page }) => {
    await withRecent(page);
    await page.route(detailRoute, (route) => route.fulfill({ status: 500 }));

    const flyout = await openFromDashboard(page);

    await expect(flyout.getByTestId('edit-transaction-load-error')).toHaveText('The transaction could not be loaded.');
    await expect(flyout.getByTestId('edit-transaction-save')).toHaveCount(0);
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldFillTheScreenWidth', async ({ page }) => {
      await withRecent(page);
      await withDetail(page);

      const flyout = await openFromDashboard(page);

      await expect(flyout.getByTestId('edit-transaction-amount')).toHaveValue('123.45');
      await expect.poll(async () => Math.round((await flyout.boundingBox())?.width ?? 0)).toBe(360);
      const overflow = await flyout.evaluate((element) => element.scrollWidth - element.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
