import tsPlugin from '@typescript-eslint/eslint-plugin';
import * as tsParser from '@typescript-eslint/parser';
import { defineConfig } from 'eslint/config';
import { importX } from 'eslint-plugin-import-x';
import eslintPluginPrettierRecommended from 'eslint-plugin-prettier/recommended';

export default defineConfig([
  {
    ignores: ['**/node_modules/**', '**/dist/**', '**/.expo/**', '**/.turbo/**'],
  },

  // TypeScript
  {
    files: ['**/*.{ts,tsx}'],
    plugins: { '@typescript-eslint': tsPlugin },
    languageOptions: { parser: tsParser },
    rules: {
      ...tsPlugin.configs.recommended.rules,
      '@typescript-eslint/no-redeclare': 'off',
    },
  },

  // Import ordering
  importX.flatConfigs.recommended,
  importX.flatConfigs.typescript,
  {
    settings: {
      'import-x/parsers': {
        '@typescript-eslint/parser': ['.ts', '.tsx'],
      },
      'import-x/resolver': {
        typescript: { alwaysTryTypes: true },
      },
    },
    rules: {
      'max-len': ['error', { code: 255 }],
      'import-x/order': [
        'error',
        {
          groups: ['builtin', 'external', 'internal', 'parent', 'sibling', 'index'],
          'newlines-between': 'never',
          alphabetize: { order: 'asc', caseInsensitive: true },
        },
      ],
    },
  },

  // Prettier
  {
    ...eslintPluginPrettierRecommended,
    rules: {
      ...eslintPluginPrettierRecommended.rules,
      'prettier/prettier': [
        'error',
        {
          semi: true,
          trailingComma: 'es5',
          singleQuote: true,
          printWidth: 144,
          tabWidth: 2,
        },
      ],
    },
  },
]);
