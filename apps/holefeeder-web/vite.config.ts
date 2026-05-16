import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { resolve } from 'path';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': resolve(__dirname, './src'),
      '@holefeeder/core': resolve(__dirname, '../../packages/core/src/index.ts'),
    },
  },
});
