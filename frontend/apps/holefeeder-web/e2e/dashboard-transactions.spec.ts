import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const transactionsRoute = '**/gateway/api/v2/dashboard/transactions';

type TransactionJson = {
  id: string;
  title: string;
  category: string | null;
  categoryColor: string;
  tags: string[];
  amount: number;
  date: string;
};

const aTransaction = (overrides: Partial<TransactionJson> = {}): TransactionJson => ({
  id: crypto.randomUUID(),
  title: 'Lunch',
  category: 'Food',
  categoryColor: '#FF8D28',
  tags: [],
  amount: 1.25,
  date: '2026-09-13',
  ...overrides,
});

const withTransactions = (page: Page, transactions: TransactionJson[]) =>
  page.route(transactionsRoute, (route) => route.fulfill({ json: transactions }));

test.describe('Dashboard transactions', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
  });

  test('ShouldShowRecentTransactionsFromApiWithBearerToken', async ({ page }) => {
    await signedIn(page);
    let authorization: string | undefined;
    await page.route(transactionsRoute, async (route) => {
      authorization = route.request().headers()['authorization'];
      await route.fulfill({
        json: [
          aTransaction({ title: 'Lunch', amount: 1.25, date: '2026-11-08' }),
          aTransaction({ title: 'Pay check', category: null, amount: 1000, date: '2026-09-13' }),
        ],
      });
    });

    await page.goto('/');

    await expect(page.getByText('Recent Transactions', { exact: true })).toBeVisible();
    const rows = page.getByTestId('dashboard-transaction-row');
    await expect(rows).toHaveCount(2);
    await expect(rows.nth(0).getByTestId('dashboard-transaction-title')).toHaveText('Lunch');
    await expect(rows.nth(0).getByTestId('dashboard-transaction-amount')).toHaveText('$1.25');
    await expect(rows.nth(0).getByTestId('dashboard-transaction-date')).toHaveText('3 days ago');
    await expect(rows.nth(1).getByTestId('dashboard-transaction-title')).toHaveText('Pay check');
    await expect(rows.nth(1).getByTestId('dashboard-transaction-amount')).toHaveText('$1,000.00');
    await expect(rows.nth(1).getByTestId('dashboard-transaction-date')).toHaveText('Sep 13, 2026');
    expect(authorization).toBe('Bearer e2e-token');
  });

  test('ShouldShowCategoryOnlyWhenTitleIsADescription', async ({ page }) => {
    await signedIn(page);
    await withTransactions(page, [aTransaction({ title: 'Lunch', category: 'Food' }), aTransaction({ title: 'Pay check', category: null })]);

    await page.goto('/');

    const rows = page.getByTestId('dashboard-transaction-row');
    await expect(rows.nth(0).getByTestId('dashboard-transaction-category')).toHaveText('Food');
    await expect(rows.nth(1).getByTestId('dashboard-transaction-title')).toHaveText('Pay check');
    await expect(rows.nth(1).getByTestId('dashboard-transaction-category')).toHaveCount(0);
  });

  test('ShouldShowTagsAsChips', async ({ page }) => {
    await signedIn(page);
    await withTransactions(page, [aTransaction({ tags: ['restaurant', 'work'] })]);

    await page.goto('/');

    await expect(page.getByTestId('dashboard-transaction-tag')).toHaveText(['restaurant', 'work']);
  });

  test('ShouldTintIconWithCategoryColour', async ({ page }) => {
    await signedIn(page);
    await withTransactions(page, [aTransaction({ categoryColor: '#FF8D28' })]);

    await page.goto('/');

    await expect(page.getByTestId('dashboard-transaction-icon')).toHaveCSS('color', 'rgb(255, 141, 40)');
  });

  test('ShouldShowEmptyStateWithoutTransactions', async ({ page }) => {
    await signedIn(page);
    await withTransactions(page, []);

    await page.goto('/');

    const empty = page.getByTestId('dashboard-transactions-empty');
    await expect(empty).toContainText('No transactions yet');
    await expect(empty).toContainText('Expenses you record will show up here.');
    await expect(page.getByTestId('dashboard-transaction-row')).toHaveCount(0);
  });

  test('ShouldShowTransactionsSkeletonUntilApiResponds', async ({ page }) => {
    await signedIn(page);
    let respond: () => void = () => {};
    await page.route(transactionsRoute, async (route) => {
      await new Promise<void>((resolve) => (respond = resolve));
      await route.fulfill({ json: [aTransaction()] });
    });

    await page.goto('/');

    await expect(page.getByTestId('dashboard-transactions-skeleton')).toBeVisible();
    respond();
    await expect(page.getByTestId('dashboard-transaction-row')).toHaveCount(1);
    await expect(page.getByTestId('dashboard-transactions-skeleton')).toHaveCount(0);
  });

  test('ShouldShowErrorWhenTransactionsFail', async ({ page }) => {
    await signedIn(page);
    await page.route(transactionsRoute, (route) => route.fulfill({ status: 500, body: 'boom' }));

    await page.goto('/');

    await expect(page.getByTestId('dashboard-transactions-error')).toHaveRole('alert');
    await expect(page.getByTestId('dashboard-transactions-error')).toHaveText('Recent transactions could not be loaded.');
    await expect(page.getByTestId('dashboard-transaction-row')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-transactions-skeleton')).toHaveCount(0);
  });
});
