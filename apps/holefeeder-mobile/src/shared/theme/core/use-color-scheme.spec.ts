import { act, renderHook } from '@testing-library/react-native';
import { useColorScheme as useRNColorScheme } from 'react-native';
import { useColorScheme } from '@/shared/theme/core/use-color-scheme.web';

jest.mock('react-native', () => ({
  useColorScheme: jest.fn(),
}));

const mockUseRNColorScheme = useRNColorScheme as jest.MockedFunction<typeof useRNColorScheme>;

describe('useColorScheme', () => {
  afterEach(() => {
    jest.clearAllMocks();
  });

  it('should return system color scheme for light theme', async () => {
    // Arrange
    mockUseRNColorScheme.mockReturnValue('light');

    // Act
    const { rerender } = renderHook(() => useColorScheme());

    // Wait for useEffect to run
    await act(async () => {
      rerender({});
    });
  });

  it('should return system color scheme after hydration for dark theme', async () => {
    // Arrange
    mockUseRNColorScheme.mockReturnValue('dark');

    // Act
    const { result, rerender } = renderHook(() => useColorScheme());

    // Wait for useEffect to run
    await act(async () => {
      rerender({});
    });

    // Assert
    expect(result.current).toBe('dark');
  });

  it('should handle null system color scheme after hydration', async () => {
    // Arrange
    mockUseRNColorScheme.mockReturnValue(null!);

    // Act
    const { result, rerender } = renderHook(() => useColorScheme());

    // Wait for useEffect to run
    await act(async () => {
      rerender({});
    });

    // Assert
    expect(result.current).toBe(null);
  });
});
