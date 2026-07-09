import { aBoolean } from '@/shared/__tests__/boolean-for-test';
import { aPositiveCount } from '@/shared/__tests__/number-for-test';
import { aUrl } from '@/shared/__tests__/string-for-test';
import { ApiConfig, ApiConfigEnvVariableNames } from '@/shared/api/api-config';

describe('ApiConfig', () => {
  const originals: Record<string, string | undefined> = {};
  const url = aUrl();
  const timeout = aPositiveCount();
  const logRequests = aBoolean();
  const simulateNetworkDelay = aPositiveCount();
  const cacheRequests = aBoolean();

  beforeAll(() => {
    for (const [key, value] of Object.entries(ApiConfigEnvVariableNames)) {
      originals[key] = process.env[value];
    }
  });

  beforeEach(() => {
    process.env[ApiConfigEnvVariableNames.apiBaseUrl] = url;
    process.env[ApiConfigEnvVariableNames.apiTimeout] = timeout.toString();
    process.env[ApiConfigEnvVariableNames.apiLogRequests] = logRequests.toString();
    process.env[ApiConfigEnvVariableNames.simulateNetworkDelay] = simulateNetworkDelay.toString();
    process.env[ApiConfigEnvVariableNames.cacheRequests] = cacheRequests.toString();
  });

  afterEach(() => {
    const restoreEnv = (env: string, originalValue?: string) => {
      if (originalValue) {
        process.env[env] = originalValue;
      } else {
        delete process.env[env];
      }
    };

    for (const [key, value] of Object.entries(ApiConfigEnvVariableNames)) {
      restoreEnv(value, originals[key]);
    }
  });

  it('succeeds with env values', () => {
    const config = ApiConfig.parseEnv();

    expect(config).toBeSuccessWithValue({
      url: url,
      timeout: timeout,
      logRequests: logRequests,
      simulateNetworkDelay: simulateNetworkDelay,
      cacheRequests: cacheRequests,
    });
  });

  it.each(Object.entries(ApiConfigEnvVariableNames))('fails when %s is not set %s', (key, value) => {
    delete process.env[value];
    const config = ApiConfig.parseEnv();

    expect(config).toBeFailureWithErrors([`env-variable-${value}-not-found`]);
  });
});
