import { expect, type Page, test } from '@playwright/test';
import { signedIn } from './support/auth';

type TagJson = { tag: string; transactionCount: number; activeCashflowCount: number };

const listRoute = '**/gateway/api/v2/tags/manage';

const car: TagJson = { tag: 'car', transactionCount: 4, activeCashflowCount: 0 };
const groceries: TagJson = { tag: 'groceries', transactionCount: 1, activeCashflowCount: 0 };
const home: TagJson = { tag: 'home', transactionCount: 12, activeCashflowCount: 1 };
const rent: TagJson = { tag: 'rent', transactionCount: 0, activeCashflowCount: 2 };

const withTags = async (page: Page, tags: TagJson[] = [car, groceries, home, rent]) => {
  let calls = 0;
  await page.route(listRoute, (route) => {
    calls += 1;
    return route.fulfill({ json: tags });
  });
  return () => calls;
};

const rowOf = (page: Page, tag: string) =>
  page.getByTestId('tags-row').filter({ has: page.getByTestId('tags-row-name').getByText(tag, { exact: true }) });

const renameRoute = '**/gateway/api/v2/tags/rename';

const capture = async (page: Page, url: string, status = 204) => {
  const bodies: unknown[] = [];
  await page.route(url, (route) => {
    bodies.push(route.request().postDataJSON());
    return route.fulfill({ status: status });
  });
  return bodies;
};

const openRename = async (page: Page, tag: string) => {
  await page.goto('/settings/tags');
  await rowOf(page, tag).getByTestId('tags-row-more').click();
  await page.getByTestId('tags-row-edit').click();
  return page.getByTestId('tag-rename-dialog');
};

const deleteRoute = '**/gateway/api/v2/tags/delete';

const chooseDelete = async (page: Page, tag: string) => {
  await page.goto('/settings/tags');
  await rowOf(page, tag).getByTestId('tags-row-more').click();
  await page.getByTestId('tags-row-delete').click();
};

test.describe('Settings tags', () => {
  test.beforeEach(async ({ page }) => {
    await signedIn(page);
  });

  test('ShouldOpenFromTheSettings', async ({ page }) => {
    await withTags(page);
    await page.route('**/gateway/api/v2/settings', (route) =>
      route.fulfill({ json: { effectiveDate: '2026-09-01', intervalType: 'monthly', frequency: 1 } })
    );
    await page.goto('/settings');

    await page.getByTestId('settings-tags').click();

    await expect(page).toHaveURL(/\/settings\/tags$/);
    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Tags');
    await expect(page.getByTestId('nav-settings')).toHaveAttribute('aria-current', 'page');
  });

  test('ShouldGoBackToTheSettings', async ({ page }) => {
    await withTags(page);
    await page.goto('/settings/tags');

    await page.getByTestId('tags-back').click();

    await expect(page).toHaveURL(/\/settings$/);
  });

  test('ShouldListTheTagsInTheOrderReceived', async ({ page }) => {
    await withTags(page);

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-row-name')).toHaveText(['car', 'groceries', 'home', 'rent']);
  });

  test('ShouldShowHowMuchEachTagIsUsed', async ({ page }) => {
    await withTags(page);

    await page.goto('/settings/tags');

    await expect(rowOf(page, 'home').getByTestId('tags-row-usage')).toHaveText('12 transactions · 1 cashflow');
    await expect(rowOf(page, 'car').getByTestId('tags-row-usage')).toHaveText('4 transactions');
    await expect(rowOf(page, 'groceries').getByTestId('tags-row-usage')).toHaveText('1 transaction');
    await expect(rowOf(page, 'rent').getByTestId('tags-row-usage')).toHaveText('2 cashflows');
  });

  test('ShouldExplainWhereTagsComeFrom_WhenThereAreNone', async ({ page }) => {
    await withTags(page, []);

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-empty')).toContainText('No tags yet');
    await expect(page.getByTestId('tags-empty')).toContainText('Tags you add to expenses show up here, ready to rename or remove.');
  });

  test('ShouldShowTheTagsLoading', async ({ page }) => {
    await page.route(listRoute, () => undefined);

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-loading')).toBeVisible();
  });

  test('ShouldReportTagsThatFailToLoad', async ({ page }) => {
    await page.route(listRoute, (route) => route.fulfill({ status: 500 }));

    await page.goto('/settings/tags');

    await expect(page.getByTestId('tags-load-error')).toHaveText('The tags could not be loaded.');
  });

  test('ShouldOfferToRenameOrDeleteATag', async ({ page }) => {
    await withTags(page);
    await page.goto('/settings/tags');

    await rowOf(page, 'groceries').getByTestId('tags-row-more').click();

    await expect(page.getByTestId('tags-row-edit')).toHaveText('Rename…');
    await expect(page.getByTestId('tags-row-delete')).toHaveText('Delete');
  });

  test('ShouldStartTheRenameFromTheCurrentName', async ({ page }) => {
    await withTags(page);

    const dialog = await openRename(page, 'groceries');

    await expect(dialog.getByRole('heading')).toHaveText('Rename groceries');
    await expect(dialog.getByTestId('tag-rename-dialog-name')).toHaveValue('groceries');
    await expect(dialog).toContainText('The new name replaces it on every transaction and cashflow. A name already in use merges both tags.');
  });

  test('ShouldOnlyRenameToANameWithoutAComma', async ({ page }) => {
    await withTags(page);
    const dialog = await openRename(page, 'groceries');
    const name = dialog.getByTestId('tag-rename-dialog-name');
    const confirm = dialog.getByTestId('tag-rename-dialog-confirm');

    await name.fill('  ');
    await expect(confirm).toBeDisabled();
    await name.fill('food,market');
    await expect(confirm).toBeDisabled();
    await name.fill('food');
    await expect(confirm).toBeEnabled();
  });

  test('ShouldRenameTheTag', async ({ page }) => {
    const listCalls = await withTags(page);
    const bodies = await capture(page, renameRoute);
    const dialog = await openRename(page, 'groceries');
    const callsBefore = listCalls();

    await dialog.getByTestId('tag-rename-dialog-name').fill(' Food ');
    await dialog.getByTestId('tag-rename-dialog-confirm').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('groceries renamed');
    await expect(dialog).toBeHidden();
    expect(bodies).toEqual([{ tag: 'groceries', newTag: 'Food' }]);
    await expect.poll(listCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldKeepTheRenameOpen_WhenItFails', async ({ page }) => {
    await withTags(page);
    await capture(page, renameRoute, 500);
    const dialog = await openRename(page, 'groceries');

    await dialog.getByTestId('tag-rename-dialog-name').fill('food');
    await dialog.getByTestId('tag-rename-dialog-confirm').click();

    await expect(dialog.getByTestId('tag-rename-dialog-error')).toHaveText('The tag could not be renamed. Try again.');
    await expect(dialog.getByTestId('tag-rename-dialog-name')).toHaveValue('food');
  });

  test('ShouldCancelTheRename', async ({ page }) => {
    await withTags(page);
    const bodies = await capture(page, renameRoute);
    const dialog = await openRename(page, 'groceries');

    await dialog.getByTestId('tag-rename-dialog-name').fill('food');
    await dialog.getByTestId('tag-rename-dialog-cancel').click();

    await expect(dialog).toBeHidden();
    expect(bodies).toEqual([]);
  });

  test('ShouldAskBeforeDeletingATag', async ({ page }) => {
    await withTags(page);

    await chooseDelete(page, 'home');

    const dialog = page.getByRole('dialog');
    await expect(dialog.getByRole('heading')).toHaveText('Delete home?');
    await expect(dialog).toContainText(
      'home is removed from 12 transactions and 1 cashflow. The transactions stay, with their amounts and their other tags.'
    );
  });

  test('ShouldNameOnlyWhatUsesTheTag_WhenAskingToDelete', async ({ page }) => {
    await withTags(page);

    await chooseDelete(page, 'groceries');

    await expect(page.getByRole('dialog')).toContainText('groceries is removed from 1 transaction.');
  });

  test('ShouldCancelTheDelete', async ({ page }) => {
    await withTags(page);
    const bodies = await capture(page, deleteRoute);
    await chooseDelete(page, 'groceries');

    await page.getByTestId('tag-delete-dialog-cancel').click();

    await expect(page.getByRole('dialog')).toBeHidden();
    expect(bodies).toEqual([]);
  });

  test('ShouldDeleteTheTag', async ({ page }) => {
    const listCalls = await withTags(page);
    const bodies = await capture(page, deleteRoute);
    await chooseDelete(page, 'groceries');
    const callsBefore = listCalls();

    await page.getByTestId('tag-delete-dialog-confirm').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('groceries deleted');
    expect(bodies).toEqual([{ tag: 'groceries' }]);
    await expect.poll(listCalls).toBeGreaterThan(callsBefore);
  });

  test('ShouldReportATagThatFailsToDelete', async ({ page }) => {
    await withTags(page);
    await capture(page, deleteRoute, 500);
    await chooseDelete(page, 'groceries');

    await page.getByTestId('tag-delete-dialog-confirm').click();

    await expect(page.getByTestId('dashboard-toast')).toHaveText('groceries could not be deleted. Try again.');
    await expect(rowOf(page, 'groceries')).toBeVisible();
  });

  test.describe('on a phone', () => {
    test.use({ viewport: { width: 360, height: 640 } });

    test('ShouldNotScrollSideways', async ({ page }) => {
      await withTags(page, [{ tag: 'a-very-long-tag-name-that-keeps-going-and-going', transactionCount: 1234, activeCashflowCount: 12 }]);

      await page.goto('/settings/tags');

      await expect(page.getByTestId('tags-row')).toBeVisible();
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow).toBe(0);
    });
  });
});
