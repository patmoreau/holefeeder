// Ensure structuredClone for environments missing it
if (typeof global.structuredClone !== 'function') {
  (global as any).structuredClone = (obj: unknown) => JSON.parse(JSON.stringify(obj));
}

// Keep test isolation tidy
afterEach(() => {
  jest.clearAllMocks();
});
