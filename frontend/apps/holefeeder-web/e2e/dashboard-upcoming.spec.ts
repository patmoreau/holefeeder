import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

const upcomingRoute = '**/gateway/api/v2/dashboard/upcoming';

type FlowJson = {
  cashflowId: string;
  date: string;
  title: string;
  category: string | null;
  categoryColor: string;
  tags: string[];
  amount: number;
  categoryType: string;
};

const aFlow = (overrides: Partial<FlowJson> = {}): FlowJson => ({
  cashflowId: crypto.randomUUID(),
  date: '2026-11-15',
  title: 'Rent',
  category: 'Housing',
  categoryColor: '#007AFF',
  tags: [],
  amount: 1450,
  categoryType: 'expense',
  ...overrides,
});

const withFlows = (page: Page, flows: FlowJson[]) => page.route(upcomingRoute, (route) => route.fulfill({ json: flows }));

test.describe('Dashboard upcoming', () => {
  test.beforeEach(async ({ page }) => {
    await page.clock.setFixedTime(new Date('2026-11-11T10:00:00'));
  });

  test('ShouldShowUpcomingFlowsFromApiWithBearerToken', async ({ page }) => {
    await signedIn(page);
    let authorization: string | undefined;
    await page.route(upcomingRoute, async (route) => {
      authorization = route.request().headers()['authorization'];
      await route.fulfill({
        json: [aFlow({ title: 'Hydro', amount: 88.4, date: '2026-11-12' }), aFlow({ title: 'Rent', amount: 1450, date: '2026-12-01' })],
      });
    });

    await page.goto('/');

    await expect(page.getByText('Upcoming Flows', { exact: true })).toBeVisible();
    const rows = page.getByTestId('dashboard-upcoming-row');
    await expect(rows).toHaveCount(2);
    await expect(rows.nth(0).getByTestId('dashboard-upcoming-title')).toHaveText('Hydro');
    await expect(rows.nth(0).getByTestId('dashboard-upcoming-amount')).toHaveText('$88.40');
    await expect(rows.nth(1).getByTestId('dashboard-upcoming-title')).toHaveText('Rent');
    await expect(rows.nth(1).getByTestId('dashboard-upcoming-amount')).toHaveText('$1,450.00');
    expect(authorization).toBe('Bearer e2e-token');
  });

  for (const [date, shown] of [
    ['2026-11-11', 'Today'],
    ['2026-11-12', 'Tomorrow'],
    ['2026-11-14', 'in 3 days'],
    ['2026-11-08', '3 days ago'],
    ['2026-12-01', 'Dec 1, 2026'],
  ]) {
    test(`ShouldShowDue_${date}_As_${shown}`, async ({ page }) => {
      await signedIn(page);
      await withFlows(page, [aFlow({ date: date })]);

      await page.goto('/');

      await expect(page.getByTestId('dashboard-upcoming-date')).toHaveText(shown);
    });
  }

  test('ShouldShowCategoryOnlyWhenTitleIsADescription', async ({ page }) => {
    await signedIn(page);
    await withFlows(page, [aFlow({ title: 'Rent', category: 'Housing' }), aFlow({ title: 'Utilities', category: null })]);

    await page.goto('/');

    const rows = page.getByTestId('dashboard-upcoming-row');
    await expect(rows.nth(0).getByTestId('dashboard-upcoming-category')).toHaveText('Housing');
    await expect(rows.nth(1).getByTestId('dashboard-upcoming-category')).toHaveCount(0);
  });

  test('ShouldShowTagsAndCategoryColour', async ({ page }) => {
    await signedIn(page);
    await withFlows(page, [aFlow({ tags: ['housing', 'home'], categoryColor: '#007AFF' })]);

    await page.goto('/');

    await expect(page.getByTestId('dashboard-upcoming-tag')).toHaveText(['housing', 'home']);
    await expect(page.getByTestId('dashboard-upcoming-icon')).toHaveCSS('color', 'rgb(0, 122, 255)');
  });

  test('ShouldShowPayButtonOnEachRow', async ({ page }) => {
    await signedIn(page);
    await withFlows(page, [aFlow(), aFlow({ title: 'Hydro' })]);

    await page.goto('/');

    const pay = page.getByTestId('dashboard-upcoming-pay');
    await expect(pay).toHaveCount(2);
    await expect(pay.first()).toHaveAccessibleName('Pay');
    await expect(pay.first()).toBeEnabled();
  });

  test('ShouldListEveryUpcomingFlow', async ({ page }) => {
    await signedIn(page);
    await withFlows(
      page,
      Array.from({ length: 7 }, (_, i) => aFlow({ title: `Flow ${i + 1}` }))
    );

    await page.goto('/');

    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(7);
  });

  test('ShouldShowEmptyStateWithoutUpcomingFlows', async ({ page }) => {
    await signedIn(page);
    await withFlows(page, []);

    await page.goto('/');

    const empty = page.getByTestId('dashboard-upcoming-empty');
    await expect(empty).toContainText('Nothing scheduled');
    await expect(empty).toContainText('Turn on Cashflow when you add an expense to repeat it on a schedule.');
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(0);
  });

  test('ShouldShowUpcomingSkeletonUntilApiResponds', async ({ page }) => {
    await signedIn(page);
    let respond: () => void = () => {};
    await page.route(upcomingRoute, async (route) => {
      await new Promise<void>((resolve) => (respond = resolve));
      await route.fulfill({ json: [aFlow()] });
    });

    await page.goto('/');

    await expect(page.getByTestId('dashboard-upcoming-skeleton')).toBeVisible();
    respond();
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(1);
    await expect(page.getByTestId('dashboard-upcoming-skeleton')).toHaveCount(0);
  });

  test('ShouldShowErrorWhenUpcomingFails', async ({ page }) => {
    await signedIn(page);
    await page.route(upcomingRoute, (route) => route.fulfill({ status: 500, body: 'boom' }));

    await page.goto('/');

    await expect(page.getByTestId('dashboard-upcoming-error')).toHaveRole('alert');
    await expect(page.getByTestId('dashboard-upcoming-error')).toHaveText('server-error');
    await expect(page.getByTestId('dashboard-upcoming-row')).toHaveCount(0);
    await expect(page.getByTestId('dashboard-upcoming-skeleton')).toHaveCount(0);
  });
});
