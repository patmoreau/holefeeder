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
