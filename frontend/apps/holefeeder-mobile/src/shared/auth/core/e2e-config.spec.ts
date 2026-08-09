import { E2eConfig } from '@/shared/auth/core/e2e-config';

describe('E2eConfig', () => {
  const original = process.env.EXPO_PUBLIC_E2E;

  afterEach(() => {
    if (original === undefined) {
      delete process.env.EXPO_PUBLIC_E2E;
    } else {
      process.env.EXPO_PUBLIC_E2E = original;
    }
  });

  it('is enabled when the flag is exactly true', () => {
    process.env.EXPO_PUBLIC_E2E = 'true';

    expect(E2eConfig.isEnabled()).toBe(true);
  });

  it('is disabled when the flag is absent', () => {
    delete process.env.EXPO_PUBLIC_E2E;

    expect(E2eConfig.isEnabled()).toBe(false);
  });

  it.each(['false', 'TRUE', '1', 'yes', ''])('is disabled when the flag is %p', (value) => {
    process.env.EXPO_PUBLIC_E2E = value;

    expect(E2eConfig.isEnabled()).toBe(false);
  });
});
