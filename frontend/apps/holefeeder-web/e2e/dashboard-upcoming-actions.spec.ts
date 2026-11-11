import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const upcomingRoute = '**/gateway/api/v2/dashboard/upcoming';
const payRoute = '**/gateway/api/v2/cashflows/pay-cashflow';

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
});
