import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const chequing = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3301', name: 'Chequing' };
const groceries = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3311', name: 'Groceries', color: '#34C759', type: 'expense' };

const transactionId = '3f2504e0-4f89-11d3-9a0c-0305e82c3321';
const transactionRoute = `**/gateway/api/v2/transactions/${transactionId}`;
const recentRoute = '**/gateway/api/v2/dashboard/transactions';

const market = {
  id: transactionId,
  date: '2026-08-28',
  amount: 123.45,
  description: 'Market',
  tags: ['weekly'],
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

const withRecent = async (page: Page) => {
  let calls = 0;
  await page.route(recentRoute, (route) => {
    calls += 1;
    return route.fulfill({ json: [row] });
  });
  return () => calls;
};

const withTransaction = async (page: Page, deleteStatus = 204) => {
  const deletes: string[] = [];
  await page.route(transactionRoute, (route) => {
    if (route.request().method() === 'DELETE') {
      deletes.push(route.request().url());
      return route.fulfill({ status: deleteStatus });
    }
    return route.fulfill({ json: market });
  });
  return deletes;
};

const withOptions = (page: Page) =>
  Promise.all([
    page.route('**/gateway/api/v2/transactions/new-transaction-options', (route) =>
      route.fulfill({ json: { accounts: [chequing], categories: [groceries] } })
    ),
    page.route('**/gateway/api/v2/transactions/tag-suggestions**', (route) => route.fulfill({ json: [] })),
  ]);

const openMenu = async (page: Page) => {
  await page.goto('/');
  await page.getByTestId('dashboard-transaction-more').click();
};

test.describe('Transaction actions', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    await withOptions(page);
  });

  test('ShouldOfferEditAndDeleteOnEachRow', async ({ page }) => {
    await withRecent(page);
    await withTransaction(page);

    await openMenu(page);

    await expect(page.getByRole('menuitem')).toHaveText(['Edit…', 'Delete']);
  });

  test('ShouldEditFromTheMenu', async ({ page }) => {
    await withRecent(page);
    await withTransaction(page);
    await openMenu(page);

    await page.getByTestId('dashboard-transaction-edit').click();

    await expect(page.getByTestId('edit-transaction-amount')).toHaveValue('123.45');
  });

  test('ShouldAskBeforeDeleting', async ({ page }) => {
    await withRecent(page);
    const deletes = await withTransaction(page);
    await openMenu(page);

    await page.getByTestId('dashboard-transaction-delete').click();

    const dialog = page.getByRole('dialog');
    await expect(dialog.getByRole('heading')).toHaveText('Delete Market?');
    await expect(dialog).toContainText('The $123.45 Market from Aug 28, 2026 leaves Chequing and its balance. This cannot be undone.');
    await page.getByTestId('transaction-delete-cancel').click();
    await expect(dialog).toHaveCount(0);
    expect(deletes).toEqual([]);
  });

  test('ShouldDeleteTheTransaction', async ({ page }) => {
    const recentCalls = await withRecent(page);
    const deletes = await withTransaction(page);
    await openMenu(page);
    const callsBefore = recentCalls();

    await page.getByTestId('dashboard-transaction-delete').click();
    await page.getByTestId('transaction-delete-confirm').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Market deleted');
    await expect(page.getByRole('dialog')).toHaveCount(0);
    expect(deletes).toHaveLength(1);
    await expect.poll(recentCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldKeepTheRowWhenDeletingFails', async ({ page }) => {
    await withRecent(page);
    await withTransaction(page, 500);
    await openMenu(page);

    await page.getByTestId('dashboard-transaction-delete').click();
    await page.getByTestId('transaction-delete-confirm').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Market could not be deleted. Try again.');
    await expect(page.getByTestId('dashboard-transaction-row')).toHaveCount(1);
  });

  test('ShouldDeleteFromTheAccountPage', async ({ page }) => {
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
    await page.route(`**/gateway/api/v2/accounts/${chequing.id}/transactions**`, (route) =>
      route.fulfill({ json: [row], headers: { 'X-Total-Count': '1' } })
    );
    const deletes = await withTransaction(page);
    await page.goto(`/accounts/${chequing.id}`);

    await page.getByTestId('account-transaction-more').click();
    await page.getByTestId('account-transaction-delete').click();
    await page.getByTestId('transaction-delete-confirm').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Market deleted');
    expect(deletes).toHaveLength(1);
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldKeepTheMenuButtonOnScreen', async ({ page }) => {
      await withRecent(page);
      await withTransaction(page);
      await page.goto('/');

      const more = await page.getByTestId('dashboard-transaction-more').boundingBox();
      expect(more!.x + more!.width).toBeLessThanOrEqual(360);
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
