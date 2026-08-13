import { isUserCancellation } from '@/shared/auth/core/is-user-cancellation';

describe('isUserCancellation', () => {
  it('recognises the error the library actually throws', () => {
    const error = Object.assign(new Error('USER_CANCELLED: The user cancelled the Web Auth operation.'), {
      type: 'USER_CANCELLED',
    });

    expect(isUserCancellation(error)).toBe(true);
  });

  it('recognises a cancellation reported on code', () => {
    expect(isUserCancellation({ code: 'USER_CANCELLED' })).toBe(true);
  });

  it('recognises a cancellation reported on name', () => {
    expect(isUserCancellation({ name: 'USER_CANCELLED' })).toBe(true);
  });

  it('does not swallow a real failure', () => {
    expect(isUserCancellation(Object.assign(new Error('boom'), { type: 'NETWORK_ERROR' }))).toBe(false);
  });

  it('handles values that are not errors at all', () => {
    expect(isUserCancellation(undefined)).toBe(false);
    expect(isUserCancellation(null)).toBe(false);
    expect(isUserCancellation('USER_CANCELLED')).toBe(false);
  });
});
