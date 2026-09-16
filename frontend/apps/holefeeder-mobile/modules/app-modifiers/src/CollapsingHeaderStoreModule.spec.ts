const mockRequireOptionalNativeModule = jest.fn();

jest.mock('expo', () => ({
  requireOptionalNativeModule: (name: string) => mockRequireOptionalNativeModule(name),
}));

const loadModule = () => {
  let module!: typeof import('./CollapsingHeaderStoreModule');
  jest.isolateModules(() => {
    // eslint-disable-next-line @typescript-eslint/no-require-imports
    module = require('./CollapsingHeaderStoreModule');
  });
  return module;
};

describe('discardCollapsingHeaderStore', () => {
  beforeEach(() => mockRequireOptionalNativeModule.mockReset());

  it('should discard the store the id names', () => {
    const discardStore = jest.fn();
    mockRequireOptionalNativeModule.mockReturnValue({ discardStore });

    const { discardCollapsingHeaderStore } = loadModule();
    discardCollapsingHeaderStore('_r_4_');

    expect(discardStore).toHaveBeenCalledWith('_r_4_');
  });

  it('should do nothing when there is no native module, so the modifiers import off a device', () => {
    mockRequireOptionalNativeModule.mockReturnValue(null);

    const { discardCollapsingHeaderStore } = loadModule();

    expect(() => discardCollapsingHeaderStore('_r_4_')).not.toThrow();
  });

  it('should do nothing when the native binary predates the function, so a reloaded bundle survives', () => {
    mockRequireOptionalNativeModule.mockReturnValue({});

    const { discardCollapsingHeaderStore } = loadModule();

    expect(() => discardCollapsingHeaderStore('_r_4_')).not.toThrow();
  });
});
