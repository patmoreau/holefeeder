import { TokenInfo } from '../core';
import { aTimestamp } from './date-for-test';
import { aToken } from './string-for-test';

const defaultTokenInfo = (): TokenInfo => ({
  token: aToken(),
  expiresAt: aTimestamp(),
});

export const aTokenInfo = (overrides?: Partial<TokenInfo>): TokenInfo => ({ ...defaultTokenInfo(), ...overrides });
