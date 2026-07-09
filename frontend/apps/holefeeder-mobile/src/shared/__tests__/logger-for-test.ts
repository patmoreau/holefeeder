export const mockDebugLogger = jest.fn();
export const mockErrorLogger = jest.fn();

jest.mock('@/shared/core/logger/logger', () => ({
  Logger: {
    create: () => ({
      debug: mockDebugLogger,
      info: mockErrorLogger,
      warn: mockErrorLogger,
      error: mockErrorLogger,
    }),
  },
}));
