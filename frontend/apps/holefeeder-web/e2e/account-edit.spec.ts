import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const accountId = '3f2504e0-4f89-11d3-9a0c-0305e82c3301';
const detailRoute = `**/gateway/api/v2/accounts/${accountId}/detail`;
const transactionsRoute = `**/gateway/api/v2/accounts/${accountId}/transactions**`;
const modifyRoute = '**/gateway/api/v2/accounts/modify-account';
const favoriteRoute = '**/gateway/api/v2/accounts/favorite-account';
const closeRoute = '**/gateway/api/v2/accounts/close-account';

const anAccount = (overrides: Record<string, unknown> = {}) => ({
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

const withAccount = async (page: Page, account = anAccount()) => {
  let detailCalls = 0;
  await page.route(detailRoute, (route) => {
    detailCalls += 1;
    return route.fulfill({ json: account });
  });
  await page.route(transactionsRoute, (route) => route.fulfill({ json: [], headers: { 'X-Total-Count': '0' } }));
  return () => detailCalls;
};

const capture = async (page: Page, url: string, status: number) => {
  const bodies: unknown[] = [];
  await page.route(url, (route) => {
    bodies.push(route.request().postDataJSON());
    return route.fulfill({ status: status });
  });
  return bodies;
};

const openEditAccount = async (page: Page) => {
  await page.goto(`/accounts/${accountId}`);
  await page.getByTestId('account-edit').click();
  return page.getByTestId('edit-account');
};

const choose = async (page: Page, field: string, option: string) => {
  await page.getByTestId(field).click();
  await page.getByRole('option', { name: option }).click();
};

test.describe('Account edit', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
  });

  test('ShouldOpenWithTheAccount', async ({ page }) => {
    await withAccount(page);

    const flyout = await openEditAccount(page);

    await expect(flyout.getByRole('heading')).toHaveText('Edit Account');
    await expect(flyout.getByTestId('edit-account-balance')).toHaveValue('500.00');
    await expect(flyout.getByTestId('edit-account-name')).toHaveValue('Chequing');
    await expect(flyout.getByTestId('edit-account-type')).toHaveText('Checking');
    await expect(flyout.getByTestId('edit-account-open-date')).toHaveValue('2021-01-04');
    await expect(flyout.getByTestId('edit-account-description')).toHaveValue('Main bank account');
    await expect(flyout.getByTestId('edit-account-favorite')).toBeChecked();
    await expect(flyout.getByTestId('edit-account-inactive')).not.toBeChecked();
    await expect(flyout.getByTestId('edit-account-save')).toHaveText('Save');
  });

  test('ShouldSaveTheChangesAndRefresh', async ({ page }) => {
    const detailCalls = await withAccount(page);
    const modified = await capture(page, modifyRoute, 204);
    const favorites = await capture(page, favoriteRoute, 204);
    const closed = await capture(page, closeRoute, 204);
    const flyout = await openEditAccount(page);
    const callsBefore = detailCalls();

    await flyout.getByTestId('edit-account-balance').fill('750.25');
    await flyout.getByTestId('edit-account-name').fill('Everyday');
    await choose(page, 'edit-account-type', 'Savings');
    await flyout.getByTestId('edit-account-open-date').fill('2020-06-15');
    await flyout.getByTestId('edit-account-description').fill('');
    await flyout.getByTestId('edit-account-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Account saved');
    expect(modified).toEqual([
      { id: accountId, name: 'Everyday', type: 'Savings', openDate: '2020-06-15', openBalance: '750.25', description: '' },
    ]);
    expect(favorites).toEqual([]);
    expect(closed).toEqual([]);
    await expect(page.getByTestId('edit-account')).toHaveCount(0);
    await expect.poll(detailCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldChangeTheFavorite', async ({ page }) => {
    await withAccount(page);
    await capture(page, modifyRoute, 204);
    const favorites = await capture(page, favoriteRoute, 204);
    const flyout = await openEditAccount(page);

    await flyout.getByTestId('edit-account-favorite').uncheck();
    await flyout.getByTestId('edit-account-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Account saved');
    expect(favorites).toEqual([{ id: accountId, isFavorite: false }]);
  });

  test('ShouldCloseTheAccount', async ({ page }) => {
    await withAccount(page);
    await capture(page, modifyRoute, 204);
    const closed = await capture(page, closeRoute, 204);
    const flyout = await openEditAccount(page);

    await expect(flyout).toContainText('Closes it. Needs no upcoming flows.');
    await flyout.getByTestId('edit-account-inactive').check();
    await flyout.getByTestId('edit-account-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Account saved');
    expect(closed).toEqual([{ id: accountId }]);
  });

  test('ShouldStayOpenWithAnError_WhenTheAccountCannotBeClosed', async ({ page }) => {
    await withAccount(page);
    await capture(page, modifyRoute, 204);
    await capture(page, closeRoute, 400);
    const flyout = await openEditAccount(page);

    await flyout.getByTestId('edit-account-inactive').check();
    await flyout.getByTestId('edit-account-save').click();

    await expect(flyout.getByTestId('edit-account-error')).toHaveText('The account could not be saved. Try again.');
    await expect(flyout).toBeVisible();
  });

  test('ShouldNotReopenAClosedAccount', async ({ page }) => {
    await withAccount(page, anAccount({ inactive: true }));

    const flyout = await openEditAccount(page);

    await expect(flyout.getByTestId('edit-account-inactive')).toBeChecked();
    await expect(flyout.getByTestId('edit-account-inactive')).toBeDisabled();
  });

  test('ShouldRequireAName', async ({ page }) => {
    await withAccount(page);
    const modified = await capture(page, modifyRoute, 204);
    const flyout = await openEditAccount(page);

    await flyout.getByTestId('edit-account-name').fill('  ');
    await flyout.getByTestId('edit-account-save').click();

    await expect(flyout).toContainText('Account name is required');
    expect(modified).toEqual([]);
  });

  test('ShouldNotOfferEditingBeforeTheAccountLoads', async ({ page }) => {
    await page.route(detailRoute, () => {});
    await page.route(transactionsRoute, () => {});

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-loading')).toBeVisible();
    await expect(page.getByTestId('account-edit')).toHaveCount(0);
  });
});

test.describe('Account edit on a phone', () => {
  test.use({ viewport: { width: 390, height: 844 } });

  test('ShouldShowAnIconWithAnAccessibleName', async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    await withAccount(page);

    await page.goto(`/accounts/${accountId}`);

    await expect(page.getByTestId('account-edit')).toHaveAccessibleName('Edit');
    const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
    expect(overflow).toBe(0);
  });
});
