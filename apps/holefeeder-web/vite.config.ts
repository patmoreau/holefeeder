import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { resolve } from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
      '@holefeeder/shared/core': resolve(__dirname, '../../packages/shared/src/core/index.ts'),
    },
  },
});
