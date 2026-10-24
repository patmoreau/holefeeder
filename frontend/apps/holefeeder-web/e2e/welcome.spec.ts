import { expect, test } from '@playwright/test';
import { authLoading, loginCalls, signedIn, signedOut } from './support/auth';

test.describe('Welcome', () => {
  test('ShouldSendSignedOutVisitorToWelcome', async ({ page }) => {
    await signedOut(page);

    await page.goto('/');

    await expect(page).toHaveURL('/welcome');
    await expect(page.getByTestId('welcome-brand-tile')).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Holefeeder' })).toBeVisible();
    await expect(page.getByText('Track where your money goes')).toBeVisible();
    await expect(page.getByText('Plan your budget, record what you spend, and see what is left.')).toBeVisible();
    await expect(page.getByTestId('welcome-signup-button')).toHaveText('Create an account');
    await expect(page.getByTestId('welcome-signin-button')).toHaveText('Sign in');
  });

  test('ShouldOpenSignUpWhenCreatingAnAccount', async ({ page }) => {
    await signedOut(page);
    await page.goto('/welcome');

    await page.getByTestId('welcome-signup-button').click();

    expect(await loginCalls(page)).toEqual([{ signUp: true }]);
  });

  test('ShouldOpenSignInWhenSigningIn', async ({ page }) => {
    await signedOut(page);
    await page.goto('/welcome');

    await page.getByTestId('welcome-signin-button').click();

    expect(await loginCalls(page)).toEqual([{ signUp: false }]);
  });

  test('ShouldShowProgressInsteadOfActionsWhileAuthenticating', async ({ page }) => {
    await authLoading(page);

    await page.goto('/welcome');

    await expect(page.getByTestId('welcome-loading')).toBeVisible();
    await expect(page.getByTestId('welcome-signup-button')).toHaveCount(0);
    await expect(page.getByTestId('welcome-signin-button')).toHaveCount(0);
  });

  test('ShouldSendSignedInUserToDashboard', async ({ page }) => {
    await signedIn(page);

    await page.goto('/welcome');

    await expect(page).toHaveURL('/');
    await expect(page.getByTestId('dashboard-page')).toBeVisible();
  });

  test('ShouldTintPrimaryActionWithLightPalette', async ({ page }) => {
    await page.emulateMedia({ colorScheme: 'light' });
    await signedOut(page);

    await page.goto('/welcome');

    await expect(page.getByTestId('welcome-signup-button')).toHaveCSS('background-color', 'rgb(123, 66, 246)');
  });

  test('ShouldTintPrimaryActionWithDarkPalette', async ({ page }) => {
    await page.emulateMedia({ colorScheme: 'dark' });
    await signedOut(page);

    await page.goto('/welcome');

    await expect(page.getByTestId('welcome-signup-button')).toHaveCSS('background-color', 'rgb(157, 109, 230)');
  });
});
