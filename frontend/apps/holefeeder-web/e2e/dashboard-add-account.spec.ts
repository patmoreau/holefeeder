import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const accountsRoute = '**/gateway/api/v2/dashboard/accounts';
const openRoute = '**/gateway/api/v2/accounts/open-account';
const favoriteRoute = '**/gateway/api/v2/accounts/favorite-account';

const newAccountId = '3f2504e0-4f89-11d3-9a0c-0305e82c3301';

const capture = async (page: Page, url: string, status: number, json?: unknown) => {
  const bodies: unknown[] = [];
  await page.route(url, (route) => {
    bodies.push(route.request().postDataJSON());
    return route.fulfill({ status: status, json: json });
  });
  return bodies;
};

const openAddAccount = async (page: Page) => {
  await page.goto('/');
  await page.getByTestId('dashboard-add-account').click();
  return page.getByTestId('add-account');
};

const choose = async (page: Page, field: string, option: string) => {
  await page.getByTestId(field).click();
  await page.getByRole('option', { name: option }).click();
};

test.describe('Dashboard add account', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    await page.route(accountsRoute, (route) => route.fulfill({ json: [] }));
  });

  test('ShouldOpenWithTheDefaults', async ({ page }) => {
    const flyout = await openAddAccount(page);

    await expect(flyout.getByRole('heading')).toHaveText('Add Account');
    await expect(flyout.getByTestId('add-account-balance')).toHaveValue('0.00');
    await expect(flyout.getByTestId('add-account-name')).toHaveValue('');
    await expect(flyout.getByTestId('add-account-type')).toHaveText('Checking');
    await expect(flyout.getByTestId('add-account-open-date')).toHaveValue('2026-11-11');
    await expect(flyout.getByTestId('add-account-description')).toHaveValue('');
    await expect(flyout.getByTestId('add-account-description')).toHaveAttribute('placeholder', 'Optional');
    await expect(flyout.getByTestId('add-account-favorite')).not.toBeChecked();
    await expect(flyout.getByTestId('add-account-inactive')).toHaveCount(0);
  });

  test('ShouldOfferEveryAccountType', async ({ page }) => {
    const flyout = await openAddAccount(page);

    await flyout.getByTestId('add-account-type').click();

    await expect(page.getByRole('option')).toHaveText(['Checking', 'Savings', 'Investment', 'Credit Card', 'Credit Line', 'Loan', 'Mortgage']);
  });

  test('ShouldOpenTheAccountAndRefreshTheDashboard', async ({ page }) => {
    const bodies = await capture(page, openRoute, 201, { id: newAccountId });
    const favorites = await capture(page, favoriteRoute, 204);
    let accountCalls = 0;
    await page.route(accountsRoute, (route) => {
      accountCalls += 1;
      return route.fallback();
    });
    const flyout = await openAddAccount(page);
    const callsBefore = accountCalls;

    await flyout.getByTestId('add-account-balance').fill('250.75');
    await flyout.getByTestId('add-account-name').fill('Travel Savings');
    await choose(page, 'add-account-type', 'Savings');
    await flyout.getByTestId('add-account-open-date').fill('2026-01-04');
    await flyout.getByTestId('add-account-description').fill('Trip to Japan');
    await flyout.getByTestId('add-account-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Account added');
    expect(bodies).toEqual([
      { type: 'Savings', name: 'Travel Savings', openDate: '2026-01-04', openBalance: '250.75', description: 'Trip to Japan' },
    ]);
    expect(favorites).toEqual([]);
    await expect(page.getByTestId('add-account')).toHaveCount(0);
    await expect.poll(() => accountCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldMarkTheNewAccountAsFavorite', async ({ page }) => {
    const bodies = await capture(page, openRoute, 201, { id: newAccountId });
    const favorites = await capture(page, favoriteRoute, 204);
    const flyout = await openAddAccount(page);

    await flyout.getByTestId('add-account-name').fill('Chequing');
    await flyout.getByTestId('add-account-favorite').check();
    await flyout.getByTestId('add-account-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Account added');
    expect(bodies).toHaveLength(1);
    expect(favorites).toEqual([{ id: newAccountId, isFavorite: true }]);
  });

  test('ShouldRequireAName', async ({ page }) => {
    const bodies = await capture(page, openRoute, 201, { id: newAccountId });
    const flyout = await openAddAccount(page);

    await flyout.getByTestId('add-account-save').click();

    await expect(flyout).toContainText('Account name is required');
    expect(bodies).toEqual([]);
    await expect(flyout).toBeVisible();
  });

  test('ShouldRefuseAnInvalidOpeningBalance', async ({ page }) => {
    const flyout = await openAddAccount(page);

    await flyout.getByTestId('add-account-name').fill('Chequing');
    await flyout.getByTestId('add-account-balance').fill('12.345');

    await expect(flyout.getByTestId('add-account-save')).toBeDisabled();
  });

  test('ShouldStayOpenWithAnError_WhenTheAccountCannotBeOpened', async ({ page }) => {
    await capture(page, openRoute, 400, { title: 'Bad Request', detail: 'Account name already exists.' });
    const flyout = await openAddAccount(page);

    await flyout.getByTestId('add-account-name').fill('Chequing');
    await flyout.getByTestId('add-account-save').click();

    await expect(flyout.getByTestId('add-account-error')).toHaveText('The account could not be saved. Try again.');
    await expect(flyout).toBeVisible();
  });

  test('ShouldCloseWithoutSaving', async ({ page }) => {
    const bodies = await capture(page, openRoute, 201, { id: newAccountId });
    const flyout = await openAddAccount(page);

    await flyout.getByTestId('add-account-name').fill('Chequing');
    await flyout.getByTestId('add-account-cancel').click();

    await expect(page.getByTestId('add-account')).toHaveCount(0);
    expect(bodies).toEqual([]);
  });

  test('ShouldNameTheCloseButton', async ({ page }) => {
    const flyout = await openAddAccount(page);

    await expect(flyout.getByTestId('add-account-close')).toHaveAccessibleName('Close');
    await flyout.getByTestId('add-account-close').click();

    await expect(page.getByTestId('add-account')).toHaveCount(0);
  });
});

test.describe('Dashboard add account on a phone', () => {
  test.use({ viewport: { width: 390, height: 844 } });

  test('ShouldFillTheScreenWithoutSidewaysScroll', async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
    await page.route(accountsRoute, (route) => route.fulfill({ json: [] }));
    const flyout = await openAddAccount(page);

    const box = await flyout.boundingBox();
    expect(box!.width).toBe(390);
    const overflow = await flyout.evaluate((element) => element.scrollWidth - element.clientWidth);
    expect(overflow).toBe(0);
  });
});
