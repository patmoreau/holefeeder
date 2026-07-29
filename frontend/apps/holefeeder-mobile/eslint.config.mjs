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
    // Vertical-slice boundaries. See docs/vertical-slice-refactor.md.
    // Applies to production code only; integration test specs may seed
    // cross-slice data in the shared PowerSync DB.
    files: ['src/**/*.{ts,tsx}'],
    ignores: ['**/*.spec.{ts,tsx}', '**/__tests__/**'],
    rules: {
      'import-x/no-restricted-paths': [
        'error',
        {
          zones: [
            {
              target: './src/summary',
              from: ['./src/accounts', './src/flows', './src/settings', './src/statistics', './src/dashboard', './src/user-registration'],
              message: 'summary/ is a neutral read-model slice consumed by view slices; it may import only shared/*.',
            },
            {
              target: ['./src/accounts', './src/flows', './src/settings', './src/user-registration'],
              from: ['./src/dashboard', './src/statistics', './src/summary'],
              message: 'A domain slice must not import a view/aggregation slice (dashboard/statistics/summary); depend the other way.',
            },
            {
              target: './src/statistics',
              from: './src/dashboard',
              message: 'statistics must not import dashboard; share via summary/ or shared/.',
            },
            {
              target: './src/dashboard',
              from: './src/statistics',
              message: 'dashboard must not import statistics; share via summary/ or shared/.',
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
