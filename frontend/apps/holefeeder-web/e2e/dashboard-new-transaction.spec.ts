import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const optionsRoute = '**/gateway/api/v2/transactions/new-transaction-options';
const tagsRoute = '**/gateway/api/v2/transactions/tag-suggestions**';
const purchaseRoute = '**/gateway/api/v2/transactions/record-purchase';
const transferRoute = '**/gateway/api/v2/transactions/record-transfer';

const chequing = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3301', name: 'Chequing' };
const savings = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3302', name: 'Savings' };
const groceries = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3311', name: 'Groceries', color: '#34C759', type: 'expense' };
const dining = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3312', name: 'Dining', color: '#FF9500', type: 'expense' };
const salary = { id: '3f2504e0-4f89-11d3-9a0c-0305e82c3313', name: 'Salary', color: '#007AFF', type: 'gain' };

const suggestions: Record<string, string[]> = {
  [groceries.id]: ['weekly', 'costco', 'family'],
  [dining.id]: ['family', 'date', 'weekly'],
  [salary.id]: ['bonus'],
};

const withOptions = async (page: Page, options = { accounts: [chequing, savings], categories: [groceries, dining, salary] }) => {
  await page.route(optionsRoute, (route) => route.fulfill({ json: options }));
  await page.route(tagsRoute, (route) => {
    const categoryId = new URL(route.request().url()).searchParams.get('categoryId') ?? '';
    return route.fulfill({ json: suggestions[categoryId] ?? [] });
  });
};

const capture = async (page: Page, url: string, status: number, json?: unknown) => {
  const bodies: unknown[] = [];
  await page.route(url, (route) => {
    bodies.push(route.request().postDataJSON());
    return route.fulfill({ status: status, json: json });
  });
  return bodies;
};

const openNewTransaction = async (page: Page) => {
  await page.goto('/');
  await page.getByTestId('dashboard-new-transaction').click();
  return page.getByTestId('new-transaction');
};

const choose = async (page: Page, field: string, option: string) => {
  await page.getByTestId(field).click();
  await page.getByRole('option', { name: option }).click();
};

test.describe('Dashboard new transaction', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
  });

  test('ShouldOpenWithTheDefaults', async ({ page }) => {
    await withOptions(page);

    const flyout = await openNewTransaction(page);

    await expect(flyout.getByRole('heading')).toHaveText('New transaction');
    await expect(flyout.getByRole('button', { name: 'Expense' })).toHaveAttribute('aria-pressed', 'true');
    await expect(flyout.getByTestId('new-transaction-amount')).toHaveValue('0.00');
    await expect(flyout.getByTestId('new-transaction-date')).toHaveValue('2026-11-11');
    await expect(flyout.getByTestId('new-transaction-account')).toHaveText('Chequing');
    await expect(flyout.getByTestId('new-transaction-category')).toHaveText('Groceries');
  });

  test('ShouldOfferTheCategoriesOfTheChosenType', async ({ page }) => {
    await withOptions(page);
    const flyout = await openNewTransaction(page);

    await flyout.getByTestId('new-transaction-category').click();
    await expect(page.getByRole('option')).toHaveText(['Groceries', 'Dining']);
    await page.keyboard.press('Escape');
    await flyout.getByRole('button', { name: 'Income' }).click();

    await expect(flyout.getByTestId('new-transaction-category')).toHaveText('Salary');
    await flyout.getByTestId('new-transaction-category').click();
    await expect(page.getByRole('option')).toHaveText(['Salary']);
  });

  test('ShouldRecordAnExpense', async ({ page }) => {
    await withOptions(page);
    const bodies = await capture(page, purchaseRoute, 201, { id: crypto.randomUUID() });
    let summaryCalls = 0;
    await page.route('**/gateway/api/v2/dashboard/summary', (route) => {
      summaryCalls += 1;
      return route.fallback();
    });
    const flyout = await openNewTransaction(page);
    const callsBefore = summaryCalls;

    await flyout.getByTestId('new-transaction-amount').fill('42.5');
    await flyout.getByTestId('new-transaction-date').fill('2026-11-10');
    await choose(page, 'new-transaction-account', 'Savings');
    await choose(page, 'new-transaction-category', 'Dining');
    await flyout.getByRole('button', { name: 'Add tag date' }).click();
    await flyout.getByTestId('new-transaction-tag-input').fill('Brunch');
    await flyout.getByTestId('new-transaction-tag-input').press('Enter');
    await flyout.getByTestId('new-transaction-description').fill('Market');
    await flyout.getByTestId('new-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transaction saved');
    expect(bodies).toEqual([
      { date: '2026-11-10', amount: '42.50', description: 'Market', accountId: savings.id, categoryId: dining.id, tags: ['date', 'Brunch'] },
    ]);
    await expect(page.getByTestId('new-transaction')).toHaveCount(0);
    await expect.poll(() => summaryCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldRecordIncome', async ({ page }) => {
    await withOptions(page);
    const bodies = await capture(page, purchaseRoute, 201, { id: crypto.randomUUID() });
    const flyout = await openNewTransaction(page);

    await flyout.getByRole('button', { name: 'Income' }).click();
    await flyout.getByTestId('new-transaction-amount').fill('2500');
    await flyout.getByTestId('new-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transaction saved');
    expect(bodies).toEqual([
      { date: '2026-11-11', amount: '2500.00', description: '', accountId: chequing.id, categoryId: salary.id, tags: [] },
    ]);
  });

  test('ShouldSuggestTagsForTheChosenCategory', async ({ page }) => {
    await withOptions(page);
    const flyout = await openNewTransaction(page);
    const suggested = flyout.getByTestId('new-transaction-suggestion');

    await expect(suggested).toHaveText(['weekly', 'costco', 'family']);
    await flyout.getByRole('button', { name: 'Add tag costco' }).click();
    await expect(flyout.getByTestId('new-transaction-tag')).toHaveText(['costco']);
    await expect(suggested).toHaveText(['weekly', 'family']);
    await choose(page, 'new-transaction-category', 'Dining');
    await expect(suggested).toHaveText(['family', 'date', 'weekly']);
    await flyout.getByRole('button', { name: 'Remove tag costco' }).click();
    await expect(flyout.getByTestId('new-transaction-tag')).toHaveCount(0);
  });

  for (const amount of ['', 'abc', '-5', '12.345']) {
    test(`ShouldNotSave_WhenAmountIs_${amount || 'empty'}`, async ({ page }) => {
      await withOptions(page);
      const flyout = await openNewTransaction(page);

      await flyout.getByTestId('new-transaction-amount').fill(amount);

      await expect(flyout.getByTestId('new-transaction-save')).toBeDisabled();
    });
  }

  test('ShouldNotSave_WhenDateIsEmpty', async ({ page }) => {
    await withOptions(page);
    const flyout = await openNewTransaction(page);

    await flyout.getByTestId('new-transaction-date').fill('');

    await expect(flyout.getByTestId('new-transaction-save')).toBeDisabled();
  });

  test('ShouldStartRepeatingOnTheTransactionDateEveryMonth', async ({ page }) => {
    await withOptions(page);
    const flyout = await openNewTransaction(page);

    await expect(flyout.getByTestId('new-transaction-repeat-starts')).toHaveCount(0);
    await flyout.getByTestId('new-transaction-date').fill('2026-11-10');
    await flyout.getByTestId('new-transaction-repeat').click();

    await expect(flyout.getByTestId('new-transaction-repeat-starts')).toHaveValue('2026-11-10');
    await expect(flyout.getByTestId('new-transaction-repeat-frequency')).toHaveValue('1');
    await expect(flyout.getByTestId('new-transaction-repeat-interval')).toHaveText('month(s)');
  });

  test('ShouldRecordARepeatingExpense', async ({ page }) => {
    await withOptions(page);
    const bodies = await capture(page, purchaseRoute, 201, { id: crypto.randomUUID() });
    const flyout = await openNewTransaction(page);

    await flyout.getByTestId('new-transaction-amount').fill('15');
    await flyout.getByTestId('new-transaction-repeat').click();
    await flyout.getByTestId('new-transaction-repeat-starts').fill('2026-11-15');
    await flyout.getByTestId('new-transaction-repeat-frequency').fill('2');
    await choose(page, 'new-transaction-repeat-interval', 'week(s)');
    await flyout.getByTestId('new-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transaction saved');
    expect(bodies).toEqual([
      {
        date: '2026-11-11',
        amount: '15.00',
        description: '',
        accountId: chequing.id,
        categoryId: groceries.id,
        tags: [],
        repeat: { startDate: '2026-11-15', intervalType: 'weekly', frequency: '2' },
      },
    ]);
  });

  test('ShouldNotRepeat_WhenRepeatIsTurnedBackOff', async ({ page }) => {
    await withOptions(page);
    const bodies = await capture(page, purchaseRoute, 201, { id: crypto.randomUUID() });
    const flyout = await openNewTransaction(page);

    await flyout.getByTestId('new-transaction-amount').fill('15');
    await flyout.getByTestId('new-transaction-repeat').click();
    await flyout.getByTestId('new-transaction-repeat').click();
    await flyout.getByTestId('new-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transaction saved');
    expect(bodies[0]).not.toHaveProperty('repeat');
  });

  for (const frequency of ['', '0', '1.5', 'abc']) {
    test(`ShouldNotSave_WhenRepeatFrequencyIs_${frequency || 'empty'}`, async ({ page }) => {
      await withOptions(page);
      const flyout = await openNewTransaction(page);

      await flyout.getByTestId('new-transaction-repeat').click();
      await flyout.getByTestId('new-transaction-repeat-frequency').fill(frequency);

      await expect(flyout.getByTestId('new-transaction-save')).toBeDisabled();
    });
  }

  test('ShouldNotSave_WhenRepeatStartIsEmpty', async ({ page }) => {
    await withOptions(page);
    const flyout = await openNewTransaction(page);

    await flyout.getByTestId('new-transaction-repeat').click();
    await flyout.getByTestId('new-transaction-repeat-starts').fill('');

    await expect(flyout.getByTestId('new-transaction-save')).toBeDisabled();
  });

  test('ShouldRecordATransferToTheNextAccount', async ({ page }) => {
    await withOptions(page);
    const bodies = await capture(page, transferRoute, 201, { fromTransactionId: crypto.randomUUID(), toTransactionId: crypto.randomUUID() });
    const flyout = await openNewTransaction(page);

    await flyout.getByRole('button', { name: 'Transfer' }).click();

    await expect(flyout.getByTestId('new-transaction-from')).toHaveText('Chequing');
    await expect(flyout.getByTestId('new-transaction-to')).toHaveText('Savings');
    await expect(flyout.getByTestId('new-transaction-category')).toHaveCount(0);
    await expect(flyout.getByTestId('new-transaction-tag-input')).toHaveCount(0);
    await expect(flyout.getByTestId('new-transaction-repeat')).toHaveCount(0);
    await flyout.getByTestId('new-transaction-amount').fill('100');
    await flyout.getByTestId('new-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Transfer saved');
    expect(bodies).toEqual([{ date: '2026-11-11', amount: '100.00', description: '', fromAccountId: chequing.id, toAccountId: savings.id }]);
    await expect(page.getByTestId('new-transaction')).toHaveCount(0);
  });

  test('ShouldSuggestTheTransferDescriptionFromTheAccounts', async ({ page }) => {
    await withOptions(page);
    const flyout = await openNewTransaction(page);

    await flyout.getByRole('button', { name: 'Transfer' }).click();

    const description = flyout.getByTestId('new-transaction-description');
    await expect(description).toHaveAttribute('placeholder', "Transfer from 'Chequing' to 'Savings'");
    await choose(page, 'new-transaction-from', 'Savings');
    await choose(page, 'new-transaction-to', 'Chequing');
    await expect(description).toHaveAttribute('placeholder', "Transfer from 'Savings' to 'Chequing'");
  });

  test('ShouldNotTransfer_WhenBothAccountsAreTheSame', async ({ page }) => {
    await withOptions(page);
    const flyout = await openNewTransaction(page);

    await flyout.getByRole('button', { name: 'Transfer' }).click();
    await flyout.getByTestId('new-transaction-amount').fill('100');
    await choose(page, 'new-transaction-to', 'Chequing');

    await expect(flyout.getByTestId('new-transaction-same-account')).toHaveText('Pick two different accounts');
    await expect(flyout.getByTestId('new-transaction-save')).toBeDisabled();
  });

  test('ShouldKeepTheFlyoutOpenAndSayItFailed_WhenSavingFails', async ({ page }) => {
    await withOptions(page);
    await capture(page, purchaseRoute, 500);
    const flyout = await openNewTransaction(page);

    await flyout.getByTestId('new-transaction-amount').fill('42.50');
    await flyout.getByTestId('new-transaction-save').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Something went wrong. Try again.');
    await expect(flyout).toBeVisible();
  });

  test('ShouldCloseWithoutSaving', async ({ page }) => {
    await withOptions(page);
    const bodies = await capture(page, purchaseRoute, 201, { id: crypto.randomUUID() });
    await page.goto('/');

    for (const close of ['new-transaction-cancel', 'new-transaction-close']) {
      await page.getByTestId('dashboard-new-transaction').click();
      await page.getByTestId(close).click();
      await expect(page.getByTestId('new-transaction')).toHaveCount(0);
    }
    expect(bodies).toEqual([]);
  });

  test('ShouldAskForAnAccountFirst_WhenThereIsNone', async ({ page }) => {
    await withOptions(page, { accounts: [], categories: [groceries] });

    const flyout = await openNewTransaction(page);

    await expect(flyout.getByTestId('new-transaction-missing')).toContainText('Add an account first');
    await expect(flyout.getByTestId('new-transaction-save')).toHaveCount(0);
  });

  test('ShouldAskForACategoryFirst_WhenThereIsNoExpenseCategory', async ({ page }) => {
    await withOptions(page, { accounts: [chequing], categories: [salary] });

    const flyout = await openNewTransaction(page);

    await expect(flyout.getByTestId('new-transaction-missing')).toContainText('Add a category first');
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 780 } });

    test('ShouldFillTheScreenWidth', async ({ page }) => {
      await withOptions(page);
      const flyout = await openNewTransaction(page);

      const box = await flyout.boundingBox();
      const save = await flyout.getByTestId('new-transaction-save').boundingBox();
      expect(box!.width).toBeCloseTo(360, 0);
      expect(save!.width).toBeGreaterThan(280);
      const overflow = await flyout.evaluate((element) => element.scrollWidth - element.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
