import { Result } from '@holefeeder/shared/core';
import { CurrentUser, UsersApi } from '@/shared/api/users-api';

const aCurrentUser = (): CurrentUser => ({ id: '1f0c8a52-6d34-4b91-8e27-5a3c9d0b7e46' });

const defaultUsersApi = (): UsersApi => ({
  getMe: () => Promise.resolve(Result.success(aCurrentUser())),
  register: () => Promise.resolve(Result.success(aCurrentUser())),
});

export const aUsersApi = (overrides?: Partial<UsersApi>): UsersApi => ({ ...defaultUsersApi(), ...overrides });
