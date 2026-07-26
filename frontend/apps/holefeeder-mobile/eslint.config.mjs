import { defineConfig } from 'eslint/config';
import expoConfig from 'eslint-config-expo/flat.js';
import rootConfig from '../../eslint.config.mjs';

export default defineConfig([
  { ignores: ['expo-env.d.ts'] },
  ...rootConfig,
  ...expoConfig,
  {
    files: ['**/*.{ts,tsx}'],
    // native/ holds the App*/Expo* wrappers; modules/ holds native SwiftUI
    // module glue — both may import @expo/ui directly.
    ignores: ['**/native/**', 'modules/**'],
    rules: {
      'no-restricted-imports': [
        'error',
        {
          patterns: [
            {
              group: ['**/native/expo/**'],
              message: 'Use the App* wrapper from native/ instead of importing Expo components directly.',
            },
            {
              group: ['@expo/ui', '@expo/ui/**'],
              message: 'Import @expo/ui only inside shared/presentation/components/native/; use an App*/Expo* wrapper elsewhere.',
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
