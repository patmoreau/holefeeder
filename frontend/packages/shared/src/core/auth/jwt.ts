import { Result } from '../result';

export const JwtErrors = {
  invalid: 'invalid-token',
};

export type JwtPayload = Record<string, unknown>;

const payload = (token: string): Result<JwtPayload> => {
  const segments = token.split('.');
  if (segments.length !== 3) return Result.failure([JwtErrors.invalid]);

  try {
    const normalized = segments[1].replace(/-/g, '+').replace(/_/g, '/');
    const claims: unknown = JSON.parse(atob(normalized));
    if (typeof claims !== 'object' || claims === null) return Result.failure([JwtErrors.invalid]);
    return Result.success(claims as JwtPayload);
  } catch {
    return Result.failure([JwtErrors.invalid]);
  }
};

export const Jwt = {
  payload: payload,
};
