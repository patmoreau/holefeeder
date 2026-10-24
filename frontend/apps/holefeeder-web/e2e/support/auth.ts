import type { LoginOptions, User } from '@holefeeder/shared/core';
import type { Page } from '@playwright/test';

type E2eAuth = {
  user?: User;
  isLoading?: boolean;
};

const aUser: User = {
  sub: 'auth0|e2e-user',
  name: 'Test User',
  givenName: 'Test',
  familyName: 'User',
  email: 'test.user@example.com',
};

const arrange = (page: Page, auth: E2eAuth) =>
  page.addInitScript((value) => {
    (window as unknown as { __E2E_AUTH__: E2eAuth }).__E2E_AUTH__ = value;
  }, auth);

export const signedOut = (page: Page) => arrange(page, {});

export const signedIn = (page: Page, user: User = aUser) => arrange(page, { user: user });

export const authLoading = (page: Page) => arrange(page, { isLoading: true });

export const loginCalls = (page: Page) =>
  page.evaluate(() => (window as unknown as { __E2E_LOGIN_CALLS__?: LoginOptions[] }).__E2E_LOGIN_CALLS__ ?? []);
