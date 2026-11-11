import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const accountId = '3f2504e0-4f89-11d3-9a0c-0305e82c3301';
const savingsId = '3f2504e0-4f89-11d3-9a0c-0305e82c3302';
const detailRoute = `**/gateway/api/v2/accounts/${accountId}/detail`;
const transactionsRoute = `**/gateway/api/v2/accounts/${accountId}/transactions**`;

const positive = 'rgb(46, 125, 50)';
const negative = 'rgb(211, 47, 47)';

type AccountDetailJson = {
  id: string;
  name: string;
  type: string;
  description: string;
  openBalance: number;
  openDate: string;
  favorite: boolean;
  inactive: boolean;
  caption: string;
  balance: number;
  upcoming: number;
  projectedBalance: number;
  updated: string;
};

const anAccount = (overrides: Partial<AccountDetailJson> = {}): AccountDetailJson => ({
  id: accountId,
  name: 'Chequing',
  type: 'Checking',
  description: 'Main bank account',
  openBalance: 500,
  openDate: '2021-01-04',
  favorite: true,
  inactive: false,
  caption: 'availableBalance',
  balance: 1986.41,
  upcoming: 970,
  projectedBalance: 2956.41,
  updated: '2026-09-13',
  ...overrides,
});

const aTransaction = (title: string, date: string, amount: number, tags: string[] = []) => ({
  id: crypto.randomUUID(),
  title: title,
  category: title,
  categoryColor: '#34C759',
  tags: tags,
  amount: amount,
  date: date,
});

const withAccount = (page: Page, account: AccountDetailJson = anAccount()) =>
  page.route(detailRoute, (route) => route.fulfill({ json: account }));

const withTransactions = (page: Page, pages: ReturnType<typeof aTransaction>[][], total: number) => {
  const requests: URL[] = [];
  return page
    .route(transactionsRoute, (route) => {
      const url = new URL(route.request().url());
      requests.push(url);
      const offset = Number(url.searchParams.get('offset'));
      const index = pages.reduce((found, _, i) => (pages.slice(0, i).flat().length === offset ? i : found), 0);
      return route.fulfill({ json: pages[index] ?? [], headers: { 'X-Total-Count': String(total) } });
    })
    .then(() => requests);
};

const withNewTransactionOptions = (page: Page) =>
  Promise.all([
    page.route('**/gateway/api/v2/transactions/new-transaction-options', (route) =>
      route.fulfill({
        json: {
          accounts: [
            { id: savingsId, name: 'Savings' },
            { id: accountId, name: 'Chequing' },
          ],
          categories: [{ id: '3f2504e0-4f89-11d3-9a0c-0305e82c3311', name: 'Groceries', color: '#34C759', type: 'expense' }],
        },
      })
    ),
    page.route('**/gateway/api/v2/transactions/tag-suggestions**', (route) => route.fulfill({ json: [] })),
  ]);

const recentTransactions = [
  aTransaction('Food', '2026-09-13', 1.25, ['restaurant']),
  aTransaction('Pay check', '2026-09-13', 1000),
  aTransaction('Gas', '2026-08-26', 54.2, ['car']),
];

test.describe('Account', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
  });

  test('ShouldShowTheAccountFromApiWithBearerToken', async ({ page }) => {
    let authorization: string | undefined;
    await page.route(detailRoute, async (route) => {
      authorization = route.request().headers()['authorization'];
      await route.fulfill({ json: anAccount() });
    });
    await withTransactions(page, [recentTransactions], 3);

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-name')).toHaveText('Chequing');
    await expect(page.getByTestId('account-caption')).toHaveText('Available Balance');
    await expect(page.getByTestId('account-balance')).toHaveText('$1,986.41');
    await expect(page.getByTestId('account-subtitle')).toHaveText('Main bank account · Checking');
    await expect(page.getByTestId('account-updated')).toHaveText('UpdatedSep 13, 2026');
    await expect(page.getByTestId('account-upcoming')).toHaveText('Upcoming$970.00');
    await expect(page.getByTestId('account-projected')).toHaveText('Projected$2,956.41');
    expect(authorization).toBe('Bearer e2e-token');
  });

  test('ShouldShowTheTypeAlone_WhenTheAccountHasNoDescription', async ({ page }) => {
    await withAccount(page, anAccount({ description: '', type: 'CreditCard' }));
    await withTransactions(page, [recentTransactions], 3);

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-subtitle')).toHaveText('Credit Card');
  });

  test('ShouldColorUpcomingAndProjectedFromTheOwnersPointOfView', async ({ page }) => {
    await withAccount(
      page,
      anAccount({ name: 'Amex', type: 'CreditCard', caption: 'amountDue', balance: -123.45, upcoming: -66.67, projectedBalance: -190.12 })
    );
    await withTransactions(page, [recentTransactions], 3);

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-caption')).toHaveText('Amount Due');
    await expect(page.getByTestId('account-balance')).toHaveText('$123.45');
    await expect(page.getByTestId('account-upcoming-amount')).toHaveText('$66.67');
    await expect(page.getByTestId('account-upcoming-amount')).toHaveCSS('color', negative);
    await expect(page.getByTestId('account-projected-amount')).toHaveText('$190.12');
    await expect(page.getByTestId('account-projected-amount')).toHaveCSS('color', negative);
  });

  test('ShouldColorAFavourableUpcomingChangeGreen', async ({ page }) => {
    await withAccount(page);
    await withTransactions(page, [recentTransactions], 3);

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-upcoming-amount')).toHaveCSS('color', positive);
    await expect(page.getByTestId('account-projected-amount')).toHaveCSS('color', positive);
  });

  test('ShouldShowASkeletonWhileTheAccountLoads', async ({ page }) => {
    await page.route(detailRoute, () => {});
    await page.route(transactionsRoute, () => {});

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-loading')).toBeVisible();
    await expect(page.getByTestId('account-transactions-skeleton')).toBeVisible();
  });

  test('ShouldShowAnError_WhenTheAccountCannotBeRead', async ({ page }) => {
    await page.route(detailRoute, (route) => route.fulfill({ status: 404 }));
    await withTransactions(page, [recentTransactions], 3);

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-error')).toBeVisible();
  });

  test('ShouldListTheTransactionsOfTheAccount', async ({ page }) => {
    await withAccount(page);
    const requests = await withTransactions(page, [recentTransactions], 3);

    await page.goto(`/accounts/${accountId}`);

    const rows = page.getByTestId('account-transaction-row');
    await expect(rows).toHaveCount(3);
    await expect(rows.nth(0).getByTestId('account-transaction-title')).toHaveText('Food');
    await expect(rows.nth(0).getByTestId('account-transaction-tag')).toHaveText('restaurant');
    await expect(rows.nth(0).getByTestId('account-transaction-amount')).toHaveText('$1.25');
    await expect(rows.nth(2).getByTestId('account-transaction-title')).toHaveText('Gas');
    await expect(page.getByTestId('account-load-more')).toHaveCount(0);
    expect(new Set(requests.map((url) => url.search))).toEqual(new Set(['?offset=0&limit=20']));
  });

  test('ShouldLoadMoreTransactions', async ({ page }) => {
    await withAccount(page);
    const firstPage = Array.from({ length: 20 }, (_, i) => aTransaction(`Purchase ${i + 1}`, '2026-09-13', 10));
    const requests = await withTransactions(page, [firstPage, [aTransaction('Oldest', '2026-01-02', 4.75)]], 21);

    await page.goto(`/accounts/${accountId}`);
    await expect(page.getByTestId('account-transaction-row')).toHaveCount(20);
    await page.getByTestId('account-load-more').click();

    await expect(page.getByTestId('account-transaction-row')).toHaveCount(21);
    await expect(page.getByTestId('account-transaction-row').nth(20).getByTestId('account-transaction-title')).toHaveText('Oldest');
    await expect(page.getByTestId('account-load-more')).toHaveCount(0);
    expect(new Set(requests.map((url) => url.search))).toEqual(new Set(['?offset=0&limit=20', '?offset=20&limit=20']));
  });

  test('ShouldShowAnError_WhenTheTransactionsCannotBeRead', async ({ page }) => {
    await withAccount(page);
    await page.route(transactionsRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-transactions-error')).toBeVisible();
  });

  test('ShouldInviteToAddAnExpense_WhenTheAccountHasNoTransactions', async ({ page }) => {
    await withAccount(page, anAccount({ name: 'Travel Savings', type: 'Savings', description: '' }));
    await withTransactions(page, [[]], 0);
    await withNewTransactionOptions(page);

    await page.goto(`/accounts/${accountId}`);
    const empty = page.getByTestId('account-transactions-empty');
    await expect(empty).toContainText('No transactions in Travel Savings');
    await expect(empty).toContainText('Record an expense or income on this account and it will appear here.');
    await page.getByTestId('account-add-expense').click();

    const flyout = page.getByTestId('new-transaction');
    await expect(flyout.getByRole('button', { name: 'Expense' })).toHaveAttribute('aria-pressed', 'true');
    await expect(flyout.getByTestId('new-transaction-account')).toHaveText('Chequing');
  });

  test('ShouldRecordATransactionOnThisAccountAndRefresh', async ({ page }) => {
    await withAccount(page);
    const requests = await withTransactions(page, [recentTransactions], 3);
    await withNewTransactionOptions(page);
    const bodies: Record<string, unknown>[] = [];
    await page.route('**/gateway/api/v2/transactions/record-purchase', (route) => {
      bodies.push(route.request().postDataJSON() as Record<string, unknown>);
      return route.fulfill({ status: 201, json: { id: crypto.randomUUID() } });
    });

    await page.goto(`/accounts/${accountId}`);
    await expect(page.getByTestId('account-transaction-row')).toHaveCount(3);
    const requestsBeforeSaving = requests.length;
    await page.getByTestId('account-new-transaction').click();
    const flyout = page.getByTestId('new-transaction');
    await expect(flyout.getByTestId('new-transaction-account')).toHaveText('Chequing');
    await flyout.getByTestId('new-transaction-amount').fill('12.50');
    await flyout.getByTestId('new-transaction-save').click();

    await expect(flyout).toBeHidden();
    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transaction saved');
    expect(bodies[0]).toMatchObject({ accountId: accountId, amount: '12.50' });
    await expect.poll(() => requests.length).toBeGreaterThan(requestsBeforeSaving);
  });

  test('ShouldGoBackToTheDashboard', async ({ page }) => {
    await withAccount(page);
    await withTransactions(page, [recentTransactions], 3);
    await page.route('**/gateway/api/v2/dashboard/**', (route) => route.fulfill({ json: [] }));

    await page.goto(`/accounts/${accountId}`);
    await expect(page.getByTestId('account-back')).toHaveAccessibleName('Back');
    await page.getByTestId('account-back').click();

    await expect(page).toHaveURL(/\/$/);
  });

  test('ShouldKeepTheDashboardSelectedInTheNavigation', async ({ page }) => {
    await withAccount(page);
    await withTransactions(page, [recentTransactions], 3);

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('nav-dashboard')).toHaveClass(/Mui-selected/);
  });
});

test.describe('Account on a phone', () => {
  test.use({ viewport: { width: 390, height: 844 } });

  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    await withAccount(page, anAccount({ name: 'Chequing account with a long name', description: 'Main bank account for every day' }));
    await withTransactions(page, [[aTransaction('Groceries at the market down the street', '2026-09-13', 1450, ['weekly', 'family'])]], 1);
  });

  test('ShouldFitWithoutSidewaysScroll', async ({ page }) => {
    await page.goto(`/accounts/${accountId}`);
    await expect(page.getByTestId('account-transaction-row')).toHaveCount(1);

    const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
    expect(overflow).toBe(0);
  });

  test('ShouldShowIconActionsWithAccessibleNames', async ({ page }) => {
    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-new-transaction')).toBeVisible();
    await expect(page.getByTestId('account-new-transaction')).toHaveAccessibleName('New transaction');
  });
});
