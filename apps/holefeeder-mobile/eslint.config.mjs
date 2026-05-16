import { defineConfig } from 'eslint/config';
import expoConfig from 'eslint-config-expo/flat.js';
import rootConfig from '../../eslint.config.mjs';

export default defineConfig([
  ...rootConfig,
  ...expoConfig,
  {
    settings: {
      react: { version: '19.2.0' },
      'import-x/resolver': {
        typescript: {
          alwaysTryTypes: true,
          project: './tsconfig.json',
        },
      },
    },
  },
]);
