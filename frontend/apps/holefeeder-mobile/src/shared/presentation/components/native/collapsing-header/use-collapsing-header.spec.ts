import { act, renderHook } from '@testing-library/react-native';
import { useCollapsingHeader } from './use-collapsing-header';

const mockDiscardStore = jest.fn();

jest.mock('@/modules/app-modifiers/src/CollapsingHeaderStoreModule', () => ({
  discardCollapsingHeaderStore: (id: string) => mockDiscardStore(id),
}));

jest.mock('react-native-safe-area-context', () => ({
  useSafeAreaInsets: () => ({ top: 60, bottom: 0, left: 0, right: 0 }),
}));

jest.mock('@/shared/theme/core/use-theme', () => ({
  useTheme: () => ({ theme: { colors: { primary: '#000000' } } }),
}));

describe('useCollapsingHeader', () => {
  beforeEach(() => mockDiscardStore.mockClear());

  it('should keep its native store while it is mounted', async () => {
    await renderHook(() => useCollapsingHeader());

    expect(mockDiscardStore).not.toHaveBeenCalled();
  });

  it('should discard its native store when it unmounts, so the registry does not grow', async () => {
    const { unmount } = await renderHook(() => useCollapsingHeader());

    await act(async () => unmount());

    expect(mockDiscardStore).toHaveBeenCalledTimes(1);
  });

  it('should discard the same id the modifiers were registered under', async () => {
    const { result, unmount } = await renderHook(() => useCollapsingHeader());
    const registeredId = (result.current.listModifiers[0] as unknown as { id: string }).id;

    await act(async () => unmount());

    expect(mockDiscardStore).toHaveBeenCalledWith(registeredId);
  });
});
