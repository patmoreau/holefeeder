import { EnvVariable } from '@/shared/core/env/env-variable';

// Guarded twice over: app.config.ts refuses to expose EXPO_PUBLIC_E2E when
// APP_ENV is production, so a production build cannot carry the flag whatever
// the env files say, and only the exact string 'true' enables it here.
const isEnabled = (): boolean => {
  const flag = EnvVariable.read('EXPO_PUBLIC_E2E');
  return flag.isSuccess && flag.value === 'true';
};

export const E2eConfig = {
  isEnabled: isEnabled,
};
