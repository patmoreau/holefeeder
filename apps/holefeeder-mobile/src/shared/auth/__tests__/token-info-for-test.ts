import { aTimestamp } from '@/shared/__tests__/date-for-test';
import { aToken } from '@/shared/__tests__/string-for-test';
import { TokenInfo } from '@/shared/auth/core/token-info';

const defaultTokenInfo = (): TokenInfo => ({
  token: aToken(),
  expiresAt: aTimestamp(),
});

export const aTokenInfo = (overrides?: Partial<TokenInfo>): TokenInfo => ({ ...defaultTokenInfo(), ...overrides });
