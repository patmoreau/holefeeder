import { EnvVariable } from './env-variable';

describe('EnvVariable', () => {
  const originalEnv = process.env;

  beforeEach(() => {
    jest.resetModules();
    process.env = { ...originalEnv };
  });

  afterAll(() => {
    process.env = originalEnv;
  });

  describe('read', () => {
    it('returns success when the environment variable is set', () => {
      const key = 'TEST_VAR';
      const value = 'test-value';
      process.env[key] = value;

      const result = EnvVariable.read(key);

      expect(result).toBeSuccessWithValue(value);
    });

    it('returns failure when the environment variable is not set', () => {
      const key = 'NON_EXISTENT_VAR';
      delete process.env[key];

      const result = EnvVariable.read(key);

      expect(result).toBeFailureWithErrors([`env-variable-${key}-not-found`]);
    });

    it('returns failure when the environment variable is an empty string', () => {
      const key = 'EMPTY_VAR';
      process.env[key] = '';

      const result = EnvVariable.read(key);

      expect(result).toBeFailureWithErrors([`env-variable-${key}-not-found`]);
    });
  });
});
