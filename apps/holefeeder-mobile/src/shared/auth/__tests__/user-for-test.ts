import { User } from '@/shared/auth/core/user';

const defaultUser = (): User => ({ sub: 'auth0|123456', email: 'test@example.com', name: 'Test User' });

export const aUser = (overrides?: Partial<User>): User => ({ ...defaultUser(), ...overrides });
