import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const accountsRoute = '**/gateway/api/v2/dashboard/accounts';

const positive = 'rgb(46, 125, 50)';
const negative = 'rgb(211, 47, 47)';
const neutral = 'rgb(28, 28, 30)';

type AccountJson = {
  id: string;
  name: string;
  caption: string;
  balance: number;
  projectedBalance: number;
  updated: string;
};

const anAccount = (overrides: Partial<AccountJson> = {}): AccountJson => ({
  id: crypto.randomUUID(),
  name: 'Chequing',
  caption: 'availableBalance',
  balance: 1986.41,
  projectedBalance: 2550,
  updated: '2026-09-13',
  ...overrides,
});

const manyAccounts = (count: number) => Array.from({ length: count }, (_, i) => anAccount({ name: `Account ${i + 1}` }));

const withAccounts = (page: Page, accounts: AccountJson[]) => page.route(accountsRoute, (route) => route.fulfill({ json: accounts }));

test.describe('Dashboard accounts', () => {
  test('ShouldShowAccountCardsFromApiWithBearerToken', async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    let authorization: string | undefined;
    await page.route(accountsRoute, async (route) => {
      authorization = route.request().headers()['authorization'];
      await route.fulfill({
        json: [
          anAccount(),
          anAccount({ name: 'Amex', caption: 'amountDue', balance: -123.45, projectedBalance: -190.12, updated: '2026-11-08' }),
        ],
      });
    });

    await page.goto('/');

    await expect(page.getByText('Accounts', { exact: true })).toBeVisible();
    const cards = page.getByTestId('dashboard-account-card');
    await expect(cards).toHaveCount(2);
    await expect(cards.nth(0).getByTestId('dashboard-account-name')).toHaveText('Chequing');
    await expect(cards.nth(0).getByTestId('dashboard-account-caption')).toHaveText('Available Balance');
    await expect(cards.nth(0).getByTestId('dashboard-account-balance')).toHaveText('$1,986.41');
    await expect(cards.nth(0).getByTestId('dashboard-account-updated')).toHaveText('UpdatedSep 13, 2026');
    await expect(cards.nth(0).getByTestId('dashboard-account-projected')).toHaveText('Projected$2,550.00');
    await expect(cards.nth(1).getByTestId('dashboard-account-name')).toHaveText('Amex');
    await expect(cards.nth(1).getByTestId('dashboard-account-caption')).toHaveText('Amount Due');
    await expect(cards.nth(1).getByTestId('dashboard-account-balance')).toHaveText('$123.45');
    await expect(cards.nth(1).getByTestId('dashboard-account-projected')).toHaveText('Projected$190.12');
    expect(authorization).toBe('Bearer e2e-token');
  });

  test('ShouldOpenTheAccountFromItsCard', async ({ page }) => {
    await signedIn(page);
    const amex = anAccount({ name: 'Amex' });
    await withAccounts(page, [anAccount(), amex]);
    await page.route(`**/gateway/api/v2/accounts/${amex.id}/**`, (route) => route.fulfill({ status: 404 }));

    await page.goto('/');
    await page.getByTestId('dashboard-account-card').nth(1).click();

    await expect(page).toHaveURL(new RegExp(`/accounts/${amex.id}$`));
    await expect(page.getByTestId('account-page')).toBeVisible();
  });

  for (const [updated, shown] of [
    ['2026-11-11', 'Today'],
    ['2026-11-10', 'Yesterday'],
    ['2026-11-08', '3 days ago'],
    ['2026-11-04', '7 days ago'],
    ['2026-11-03', 'Nov 3, 2026'],
  ]) {
    test(`ShouldShowUpdated_${updated}_As_${shown}`, async ({ page }) => {
      await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
      await signedIn(page);
      await withAccounts(page, [anAccount({ updated: updated })]);

      await page.goto('/');

      await expect(page.getByTestId('dashboard-account-updated')).toHaveText(`Updated${shown}`);
    });
  }

  test('ShouldShowCreditBalanceCaption', async ({ page }) => {
    await signedIn(page);
    await withAccounts(page, [anAccount({ name: 'Visa', caption: 'creditBalance', balance: 20 })]);

    await page.goto('/');

    await expect(page.getByTestId('dashboard-account-caption')).toHaveText('Credit Balance');
  });

  test('ShouldColourBalancesByTone', async ({ page }) => {
    await signedIn(page);
    await withAccounts(page, [
      anAccount({ balance: 1986.41, projectedBalance: -50 }),
      anAccount({ caption: 'amountDue', balance: -123.45, projectedBalance: 0.004 }),
    ]);

    await page.goto('/');

    const cards = page.getByTestId('dashboard-account-card');
    await expect(cards.nth(0).getByTestId('dashboard-account-balance')).toHaveCSS('color', positive);
    await expect(cards.nth(0).getByTestId('dashboard-account-projected-amount')).toHaveCSS('color', negative);
    await expect(cards.nth(1).getByTestId('dashboard-account-balance')).toHaveCSS('color', negative);
    await expect(cards.nth(1).getByTestId('dashboard-account-projected-amount')).toHaveText('$0.00');
    await expect(cards.nth(1).getByTestId('dashboard-account-projected-amount')).toHaveCSS('color', neutral);
  });

  test('ShouldShowDisabledAddAccountTileAfterTheCards', async ({ page }) => {
    await signedIn(page);
    await withAccounts(page, [anAccount(), anAccount({ name: 'Savings' })]);

    await page.goto('/');

    const add = page.getByTestId('dashboard-add-account');
    await expect(add).toHaveAccessibleName('Add an account');
    await expect(add).toBeDisabled();
    const lastCard = await page.getByTestId('dashboard-account-card').nth(1).boundingBox();
    const addBox = await add.boundingBox();
    expect(addBox!.x).toBeGreaterThan(lastCard!.x + lastCard!.width - 1);
  });

  test('ShouldShowOnlyAddAccountTileWithoutAccounts', async ({ page }) => {
    await signedIn(page);
    await withAccounts(page, []);

    await page.goto('/');

    await expect(page.getByTestId('dashboard-add-account')).toBeVisible();
    await expect(page.getByTestId('dashboard-account-card')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-account-skeleton')).toHaveCount(0);
  });

  test('ShouldListAccountsOnASingleScrollableRow', async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 800 });
    await signedIn(page);
    await withAccounts(page, manyAccounts(8));

    await page.goto('/');

    const cards = page.getByTestId('dashboard-account-card');
    await expect(cards).toHaveCount(8);
    const first = await cards.first().boundingBox();
    const last = await cards.last().boundingBox();
    expect(last!.y).toBe(first!.y);
    const row = page.getByTestId('dashboard-accounts-row');
    expect(await row.evaluate((el) => el.scrollWidth > el.clientWidth)).toBe(true);
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
  });

  test('ShouldHideArrowsWhenAllAccountsFit', async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 800 });
    await signedIn(page);
    await withAccounts(page, manyAccounts(2));

    await page.goto('/');

    await expect(page.getByTestId('dashboard-account-card')).toHaveCount(2);
    await expect(page.getByTestId('dashboard-accounts-previous')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-accounts-next')).toHaveCount(0);
  });

  test('ShouldScrollAccountsWithArrows', async ({ page }) => {
    await page.setViewportSize({ width: 1280, height: 800 });
    await signedIn(page);
    await withAccounts(page, manyAccounts(8));

    await page.goto('/');

    const row = page.getByTestId('dashboard-accounts-row');
    const previous = page.getByTestId('dashboard-accounts-previous');
    const next = page.getByTestId('dashboard-accounts-next');
    await expect(previous).toHaveAccessibleName('Previous accounts');
    await expect(next).toHaveAccessibleName('Next accounts');
    await expect(previous).toBeDisabled();
    await expect(next).toBeEnabled();

    await next.click();
    await expect.poll(() => row.evaluate((el) => el.scrollLeft)).toBeGreaterThan(0);
    await expect(previous).toBeEnabled();

    await row.evaluate((el) => el.scrollTo({ left: el.scrollWidth }));
    await expect(next).toBeDisabled();

    await previous.click();
    await expect(next).toBeEnabled();
  });

  test('ShouldShowAccountSkeletonsUntilApiResponds', async ({ page }) => {
    await signedIn(page);
    let respond: () => void = () => {};
    await page.route(accountsRoute, async (route) => {
      await new Promise<void>((resolve) => (respond = resolve));
      await route.fulfill({ json: [anAccount()] });
    });

    await page.goto('/');

    await expect(page.getByTestId('dashboard-account-skeleton')).toHaveCount(4);
    respond();
    await expect(page.getByTestId('dashboard-account-card')).toHaveCount(1);
    await expect(page.getByTestId('dashboard-account-skeleton')).toHaveCount(0);
  });

  test('ShouldShowErrorWhenAccountsFail', async ({ page }) => {
    await signedIn(page);
    await page.route(accountsRoute, (route) => route.fulfill({ status: 500, body: 'boom' }));

    await page.goto('/');

    await expect(page.getByTestId('dashboard-accounts-error')).toHaveRole('alert');
    await expect(page.getByTestId('dashboard-account-card')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-account-skeleton')).toHaveCount(0);
  });
});
