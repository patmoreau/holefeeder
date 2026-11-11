import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const upcomingRoute = '**/gateway/api/v2/dashboard/upcoming';
const payRoute = '**/gateway/api/v2/cashflows/pay-cashflow';
const clearRoute = '**/gateway/api/v2/cashflows/clear-cashflow';
const cancelRoute = '**/gateway/api/v2/cashflows/cancel-cashflow';

const rent = {
  cashflowId: '3f2504e0-4f89-11d3-9a0c-0305e82c3301',
  date: '2026-11-15',
  title: 'Rent',
  category: 'Housing',
  categoryColor: '#007AFF',
  tags: [],
  amount: 1450,
  categoryType: 'expense',
};

const capture = async (page: Page, url: string, status: number, json?: unknown) => {
  const bodies: unknown[] = [];
  await page.route(url, (route) => {
    bodies.push(route.request().postDataJSON());
    return route.fulfill({ status: status, json: json });
  });
  return bodies;
};

const withUpcomingUntilActedOn = async (page: Page, actions: unknown[]) => {
  let callsAfterAction = 0;
  await page.route(upcomingRoute, (route) => {
    if (actions.length > 0) callsAfterAction += 1;
    return route.fulfill({ json: actions.length > 0 ? [] : [rent] });
  });
  return () => callsAfterAction;
};

test.describe('Dashboard upcoming actions', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
    await signedIn(page);
  });

  test('ShouldPayAsScheduled', async ({ page }) => {
    const bodies = await capture(page, payRoute, 201, { id: crypto.randomUUID() });
    const refetches = await withUpcomingUntilActedOn(page, bodies);
    await page.goto('/');

    await page.getByTestId('dashboard-upcoming-pay').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Rent paid');
    expect(bodies).toEqual([
      { cashflowId: rent.cashflowId, date: '2026-11-15', amount: '1450.00', cashflowDate: '2026-11-15', updateRecurringAmount: 'false' },
    ]);
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(0);
    expect(refetches()).toBeGreaterThan(0);
  });

  test('ShouldRefreshTheOtherSectionsAfterPaying', async ({ page }) => {
    const bodies = await capture(page, payRoute, 201, { id: crypto.randomUUID() });
    await withUpcomingUntilActedOn(page, bodies);
    await page.route('**/gateway/api/v2/dashboard/summary', (route) =>
      route.fulfill({
        json: {
          currentExpenses: bodies.length > 0 ? 1550 : 100,
          expenseVariation: 0,
          expenseVariationPercentage: 0,
          netFlow: 0,
          currentGains: 0,
          averageExpenses: 0,
          projectedNetFlow: 0,
        },
      })
    );
    await page.goto('/');
    await expect(page.getByTestId('dashboard-current-spending')).toHaveText('$100.00');

    await page.getByTestId('dashboard-upcoming-pay').click();

    await expect(page.getByTestId('dashboard-current-spending')).toHaveText('$1,550.00');
    await expect(page.getByTestId('dashboard-spending-skeleton')).toHaveCount(0);
  });

  test('ShouldKeepTheRowAndSayItFailed_WhenPayingFails', async ({ page }) => {
    await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
    await capture(page, payRoute, 500);
    await page.goto('/');

    await page.getByTestId('dashboard-upcoming-pay').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Something went wrong. Try again.');
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(1);
  });

  test('ShouldOfferTheOtherActionsInAMenu', async ({ page }) => {
    await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
    await page.goto('/');

    const more = page.getByTestId('dashboard-upcoming-more');
    await expect(more).toHaveAccessibleName('More actions');
    await more.click();

    await expect(page.getByRole('menuitem')).toHaveText(['Pay with changes…', 'Clear', 'Delete']);
  });

  test('ShouldClearTheOccurrence', async ({ page }) => {
    const bodies = await capture(page, clearRoute, 201, { id: crypto.randomUUID() });
    await withUpcomingUntilActedOn(page, bodies);
    await page.goto('/');

    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-clear').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Rent cleared for Nov 15, 2026');
    expect(bodies).toEqual([{ cashflowId: rent.cashflowId, cashflowDate: '2026-11-15' }]);
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(0);
  });

  test('ShouldAskBeforeDeleting', async ({ page }) => {
    const bodies = await capture(page, cancelRoute, 204);
    await withUpcomingUntilActedOn(page, bodies);
    await page.goto('/');

    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-delete').click();

    const dialog = page.getByRole('dialog');
    await expect(dialog.getByRole('heading')).toHaveText('Delete Rent?');
    await expect(dialog).toContainText('Rent stops repeating and leaves your upcoming flows. Payments already made stay in your transactions.');
    await page.getByTestId('dashboard-delete-cancel').click();
    await expect(dialog).toHaveCount(0);
    expect(bodies).toEqual([]);
  });

  test('ShouldDeleteTheCashflowOnceConfirmed', async ({ page }) => {
    const bodies = await capture(page, cancelRoute, 204);
    await withUpcomingUntilActedOn(page, bodies);
    await page.goto('/');

    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-delete').click();
    await page.getByTestId('dashboard-delete-confirm').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Rent deleted');
    expect(bodies).toEqual([{ cashflowId: rent.cashflowId }]);
    await expect(page.getByRole('dialog')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(0);
  });

  test('ShouldPrefillTheFlyoutWithTheScheduledPayment', async ({ page }) => {
    await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
    await page.goto('/');

    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-pay-with-changes').click();

    const flyout = page.getByTestId('pay-flyout');
    await expect(flyout.getByRole('heading')).toHaveText('Pay Rent');
    await expect(flyout.getByTestId('pay-flyout-subtitle')).toHaveText('Housing · Due Nov 15, 2026');
    await expect(flyout.getByTestId('pay-flyout-amount')).toHaveValue('1450.00');
    await expect(flyout.getByTestId('pay-flyout-date')).toHaveValue('2026-11-15');
    await expect(flyout.getByTestId('pay-flyout-scheduled')).toHaveCount(0);
    await expect(flyout.getByTestId('pay-flyout-update-recurring')).toHaveCount(0);
  });

  test('ShouldPayWithChangedAmountAndDate', async ({ page }) => {
    const bodies = await capture(page, payRoute, 201, { id: crypto.randomUUID() });
    await withUpcomingUntilActedOn(page, bodies);
    await page.goto('/');
    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-pay-with-changes').click();

    const flyout = page.getByTestId('pay-flyout');
    await flyout.getByTestId('pay-flyout-amount').fill('1500');
    await flyout.getByTestId('pay-flyout-date').fill('2026-11-14');
    await expect(flyout.getByTestId('pay-flyout-scheduled')).toHaveText('Scheduled $1,450.00');
    const updateRecurring = flyout.getByRole('checkbox', { name: 'Update recurring amount to $1,500.00 going forward' });
    await expect(updateRecurring).not.toBeChecked();
    await updateRecurring.check();
    await flyout.getByTestId('pay-flyout-pay').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Rent paid');
    expect(bodies).toEqual([
      { cashflowId: rent.cashflowId, date: '2026-11-14', amount: '1500.00', cashflowDate: '2026-11-15', updateRecurringAmount: 'true' },
    ]);
    await expect(flyout).toHaveCount(0);
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(0);
  });

  test('ShouldNotUpdateRecurringAmount_WhenAmountIsBackToScheduled', async ({ page }) => {
    const bodies = await capture(page, payRoute, 201, { id: crypto.randomUUID() });
    await withUpcomingUntilActedOn(page, bodies);
    await page.goto('/');
    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-pay-with-changes').click();

    const flyout = page.getByTestId('pay-flyout');
    await flyout.getByTestId('pay-flyout-amount').fill('1500');
    await flyout.getByRole('checkbox').check();
    await flyout.getByTestId('pay-flyout-amount').fill('1450');
    await flyout.getByTestId('pay-flyout-pay').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Rent paid');
    expect(bodies).toEqual([
      { cashflowId: rent.cashflowId, date: '2026-11-15', amount: '1450.00', cashflowDate: '2026-11-15', updateRecurringAmount: 'false' },
    ]);
  });

  for (const amount of ['', 'abc', '-5', '12.345']) {
    test(`ShouldNotPay_WhenAmountIs_${amount || 'empty'}`, async ({ page }) => {
      await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
      await page.goto('/');
      await page.getByTestId('dashboard-upcoming-more').click();
      await page.getByTestId('dashboard-upcoming-pay-with-changes').click();

      await page.getByTestId('pay-flyout-amount').fill(amount);

      await expect(page.getByTestId('pay-flyout-pay')).toBeDisabled();
    });
  }

  test('ShouldNotPay_WhenDateIsEmpty', async ({ page }) => {
    await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
    await page.goto('/');
    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-pay-with-changes').click();

    await page.getByTestId('pay-flyout-date').fill('');

    await expect(page.getByTestId('pay-flyout-pay')).toBeDisabled();
  });

  test('ShouldCloseTheFlyoutWithoutPaying', async ({ page }) => {
    const bodies = await capture(page, payRoute, 201, { id: crypto.randomUUID() });
    await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
    await page.goto('/');

    for (const close of ['pay-flyout-cancel', 'pay-flyout-close']) {
      await page.getByTestId('dashboard-upcoming-more').click();
      await page.getByTestId('dashboard-upcoming-pay-with-changes').click();
      await page.getByTestId(close).click();
      await expect(page.getByTestId('pay-flyout')).toHaveCount(0);
    }
    expect(bodies).toEqual([]);
  });

  test('ShouldKeepTheFlyoutOpenAndSayItFailed_WhenPayingFails', async ({ page }) => {
    await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
    await capture(page, payRoute, 500);
    await page.goto('/');
    await page.getByTestId('dashboard-upcoming-more').click();
    await page.getByTestId('dashboard-upcoming-pay-with-changes').click();

    await page.getByTestId('pay-flyout-pay').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('Something went wrong. Try again.');
    await expect(page.getByTestId('pay-flyout')).toBeVisible();
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 780 } });

    test('ShouldFillTheScreenWidth', async ({ page }) => {
      await page.route(upcomingRoute, (route) => route.fulfill({ json: [rent] }));
      await page.goto('/');
      await page.getByTestId('dashboard-upcoming-more').click();
      await page.getByTestId('dashboard-upcoming-pay-with-changes').click();

      const flyout = await page.getByTestId('pay-flyout').boundingBox();
      const pay = await page.getByTestId('pay-flyout-pay').boundingBox();
      const cancel = await page.getByTestId('pay-flyout-cancel').boundingBox();
      expect(flyout!.width).toBeCloseTo(360, 0);
      expect(pay!.width).toBeGreaterThan(280);
      expect(cancel!.width).toBe(pay!.width);
    });
  });
});
