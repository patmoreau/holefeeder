import { defineConfig } from 'eslint/config';
import react from 'eslint-plugin-react';
import reactHooks from 'eslint-plugin-react-hooks';
import rootConfig from '../../eslint.config.mjs';

export default defineConfig([
  ...rootConfig,
  {
    files: ['**/*.tsx'],
    ...react.configs.flat.recommended,
  },
  {
    files: ['**/*.tsx'],
    ...react.configs.flat['jsx-runtime'],
  },
  {
    files: ['**/*.{ts,tsx}'],
    ...reactHooks.configs.flat['recommended-latest'],
  },
  {
    settings: { react: { version: '19.3.0' } },
  },
]);
