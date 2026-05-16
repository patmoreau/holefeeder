import './globals';
import './result-matcher';

// If you have one-off test helpers or expect extensions, import them here too:
// import '@testing-library/jest-native/extend-expect';

jest.mock('module');

jest.mock('expo-crypto', () => ({
  // eslint-disable-next-line @typescript-eslint/no-require-imports
  randomUUID: () => require('crypto').randomUUID(),
}));
