import { defineConfig } from 'eslint/config';
import expoConfig from 'eslint-config-expo/flat.js';
import rootConfig from '../../eslint.config.mjs';

export default defineConfig([
  { ignores: ['expo-env.d.ts'] },
  ...rootConfig,
  ...expoConfig,
  {
    files: ['**/*.{ts,tsx}'],
    ignores: ['**/native/**'],
    rules: {
      'no-restricted-imports': [
        'error',
        {
          patterns: [
            {
              group: ['**/native/expo/**'],
              message: 'Use the App* wrapper from native/ instead of importing Expo components directly.',
            },
          ],
        },
      ],
    },
  },
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
    rules: {
      '@typescript-eslint/no-redeclare': 'off',
      'import/no-unresolved': 'off',
      'import-x/no-unresolved': 'off',
    },
  },
]);
