import type { Config } from 'jest';

const config: Config = {
  preset: 'ts-jest',
  testEnvironment: 'node',
  moduleFileExtensions: ['ts', 'js'],
  testMatch: ['**/*.spec.ts'],
  transform: {
    '^.+\\.[tj]s$': ['ts-jest', { tsconfig: './tsconfig.test.json' }],
  },
  transformIgnorePatterns: ['node_modules/(?!(\\.pnpm/[^/]+/node_modules/)?@faker-js/)'],
  setupFilesAfterEnv: ['<rootDir>/src/testkit/result-matcher.ts'],
};

export default config;
