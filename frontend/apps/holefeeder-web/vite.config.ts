import { resolve } from 'path';
import react from '@vitejs/plugin-react';
import { defineConfig } from 'vite';

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: [
      { find: /^@holefeeder\/shared\/(.*)$/, replacement: resolve(__dirname, '../../packages/shared/src/$1/index.ts') },
      { find: '@', replacement: resolve(__dirname, './src') },
    ],
  },
});
