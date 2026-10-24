import { expect, test } from '@playwright/test';
import { signedIn } from './support/auth';

const anAccount = (overrides: Record<string, unknown>) => ({
  id: '0b6c1a1e-6a3f-4c38-9a3e-2f1d1b8f5c01',
  type: 'Checking',
  name: 'Chequing',
  openBalance: 1000,
  openDate: '2026-01-01',
  description: 'Everyday account',
  favorite: false,
  inactive: false,
  ...overrides,
});

test.describe('Accounts', () => {
  test('ShouldListAccountsFromApiWithBearerToken', async ({ page }) => {
    await signedIn(page);
    let authorization: string | undefined;
    await page.route('**/gateway/api/v2/accounts', async (route) => {
      authorization = route.request().headers()['authorization'];
      await route.fulfill({
        json: [anAccount({}), anAccount({ id: '0b6c1a1e-6a3f-4c38-9a3e-2f1d1b8f5c02', type: 'CreditCard', name: 'Amex', description: '' })],
      });
    });

    await page.goto('/accounts');

    await expect(page.getByText('Chequing')).toBeVisible();
    await expect(page.getByText('Everyday account')).toBeVisible();
    await expect(page.getByText('Amex')).toBeVisible();
    expect(authorization).toBe('Bearer e2e-token');
  });

  test('ShouldShowProgressUntilApiResponds', async ({ page }) => {
    await signedIn(page);
    let respond: () => void = () => {};
    await page.route('**/gateway/api/v2/accounts', async (route) => {
      await new Promise<void>((resolve) => (respond = resolve));
      await route.fulfill({ json: [anAccount({})] });
    });

    await page.goto('/accounts');

    await expect(page.getByRole('progressbar')).toBeVisible();
    respond();
    await expect(page.getByText('Chequing')).toBeVisible();
    await expect(page.getByRole('progressbar')).toHaveCount(0);
  });

  test('ShouldShowErrorWhenApiFails', async ({ page }) => {
    await signedIn(page);
    await page.route('**/gateway/api/v2/accounts', (route) => route.fulfill({ status: 500, body: 'boom' }));

    await page.goto('/accounts');

    await expect(page.getByRole('alert')).toBeVisible();
    await expect(page.getByText('Chequing')).toHaveCount(0);
  });

  test('ShouldShowEmptyStateWithoutAccounts', async ({ page }) => {
    await signedIn(page);
    await page.route('**/gateway/api/v2/accounts', (route) => route.fulfill({ json: [] }));

    await page.goto('/accounts');

    await expect(page.getByText('No accounts found.')).toBeVisible();
  });
});
