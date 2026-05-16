import type { Config } from 'jest';

const config: Config = {
  preset: 'ts-jest',
  testEnvironment: 'node',
  moduleFileExtensions: ['ts', 'js'],
  testMatch: ['**/*.spec.ts'],
  transform: {
    '^.+\\.ts$': ['ts-jest', { tsconfig: { module: 'CommonJS', types: ['node', 'jest'] } }],
  },
  setupFilesAfterEnv: ['<rootDir>/src/core/__tests__/setup/result-matcher.ts'],
};

export default config;
