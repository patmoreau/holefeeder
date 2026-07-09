import { resolve } from 'path';
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
      '@holefeeder/shared/core': resolve(__dirname, '../../packages/shared/src/core/index.ts'),
    },
  },
});
