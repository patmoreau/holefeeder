import { Jwt, type TokenInfo } from '@holefeeder/shared/core';

const info = (token: string): TokenInfo | undefined => {
  const payload = Jwt.payload(token);
  if (payload.isFailure || typeof payload.value.exp !== 'number') return undefined;
  return { token: token, expiresAt: payload.value.exp };
};

export const AccessToken = {
  info: info,
};
