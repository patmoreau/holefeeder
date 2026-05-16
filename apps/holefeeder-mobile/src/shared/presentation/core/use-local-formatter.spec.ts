import { act, renderHook } from '@testing-library/react-native';
import * as ExpoLocalization from 'expo-localization';
import { AppState, AppStateStatus } from 'react-native';
import { DateOnly } from '@/shared/core/date-only';
import { withDate } from '@/shared/core/with-date';
import { useLocaleFormatter } from '@/shared/presentation/core/use-local-formatter';

jest.mock('expo-localization');

describe('useLocaleFormatter', () => {
  const mockLocale = {
    languageTag: 'en-US',
    currencyCode: 'USD',
  };

  const mockFrenchLocale = {
    languageTag: 'fr-CA',
    currencyCode: 'CAD',
  };

  beforeEach(() => {
    jest.clearAllMocks();
    (ExpoLocalization.getLocales as jest.Mock).mockReturnValue([mockLocale]);
  });

  describe('initialization', () => {
    it('should return current locale and currency code', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      expect(result.current.currentLocale).toBe('en-US');
      expect(result.current.currencyCode).toBe('USD');
    });

    it('should default to CAD when currencyCode is null', () => {
      (ExpoLocalization.getLocales as jest.Mock).mockReturnValue([{ languageTag: 'en-US', currencyCode: null }]);

      const { result } = renderHook(() => useLocaleFormatter());

      expect(result.current.currencyCode).toBe('CAD');
    });

    it('should return formatter functions', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      expect(typeof result.current.formatCurrency).toBe('function');
      expect(typeof result.current.formatDate).toBe('function');
      expect(typeof result.current.formatPercentage).toBe('function');
    });
  });

  describe('formatCurrency', () => {
    it('should format amount with default currency code', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatCurrency(1234.56);

      expect(formatted).toBe('$1,234.56');
    });

    it('should format amount with custom currency', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatCurrency(1234.56, { currency: 'EUR' });

      expect(formatted).toBe('€1,234.56');
    });

    it('should fallback to CAD when currency is undefined', () => {
      (ExpoLocalization.getLocales as jest.Mock).mockReturnValue([{ languageTag: 'en-US', currencyCode: undefined }]);

      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatCurrency(1234.56);

      expect(formatted).toBe('CA$1,234.56');
    });

    it('should handle invalid currency gracefully', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatCurrency(1234.56, { currency: '' });

      expect(formatted).toBe('CA$1,234.56');
    });

    it('should fallback to string format on error', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      // Mock Intl.NumberFormat to throw an error
      const originalNumberFormat = Intl.NumberFormat;
      Intl.NumberFormat = jest.fn().mockImplementation(() => {
        throw new Error('Invalid currency');
      }) as any;

      const formatted = result.current.formatCurrency(1234.56, { currency: 'INVALID' });

      expect(formatted).toBe('1234.56 INVALID');

      // Restore original
      Intl.NumberFormat = originalNumberFormat;
    });
  });

  describe('formatDate', () => {
    it('should format DateOnly object', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2025-12-25');

      const formatted = result.current.formatDate(date, anchor);

      expect(formatted).toMatch(/Dec/);
      expect(formatted).toMatch(/2023/);
      expect(formatted.length).toBeGreaterThan(0);
    });

    it('should accept custom format options', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2025-12-25');

      const formatted = result.current.formatDate(date, anchor, { dateStyle: 'short' });

      expect(formatted).toMatch(/12/);
      expect(formatted).toMatch(/2023|23/);
      expect(formatted.length).toBeGreaterThan(0);
    });

    it('should display today', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2023-12-25');

      const formatted = result.current.formatDate(date, anchor);

      expect(formatted).toMatch('common.today');
    });

    it('should display yesterday', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-24');
      const anchor = DateOnly.valid('2023-12-25');

      const formatted = result.current.formatDate(date, anchor);

      expect(formatted).toMatch('common.yesterday');
    });

    it('should display n days ago', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-24');
      const anchor = DateOnly.valid('2023-12-31');

      const formatted = result.current.formatDate(date, anchor);

      expect(formatted).toMatch('common.last7Days');
    });

    it('should display tomorrow', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-26');
      const anchor = DateOnly.valid('2023-12-25');

      const formatted = result.current.formatDate(date, anchor);

      expect(formatted).toMatch('common.tomorrow');
    });

    it('should display in next n days', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-31');
      const anchor = DateOnly.valid('2023-12-24');

      const formatted = result.current.formatDate(date, anchor);

      expect(formatted).toMatch('common.next7Days');
    });

    it('should fallback to toDateString on error', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const date = DateOnly.valid('2023-12-25');
      const anchor = DateOnly.valid('2025-12-24');

      // Mock Intl.DateTimeFormat to throw an error
      const originalDateTimeFormat = Intl.DateTimeFormat;
      Intl.DateTimeFormat = jest.fn().mockImplementation(() => {
        throw new Error('Invalid format');
      }) as any;

      const formatted = result.current.formatDate(date, anchor);

      expect(formatted).toBe(withDate(date).toDate().toDateString());

      // Restore original
      Intl.DateTimeFormat = originalDateTimeFormat;
    });
  });

  describe('formatPercentage', () => {
    it('should format percentage with 2 decimal places', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatPercentage(12.3456);

      expect(formatted).toBe('12.35%');
    });

    it('should handle whole numbers', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatPercentage(50);

      expect(formatted).toBe('50.00%');
    });

    it('should handle zero', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatPercentage(0);

      expect(formatted).toBe('0.00%');
    });

    it('should handle negative values', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const formatted = result.current.formatPercentage(-5.678);

      expect(formatted).toBe('-5.68%');
    });
  });

  describe('locale change detection', () => {
    let appStateListeners: ((state: AppStateStatus) => void)[] = [];

    beforeEach(() => {
      appStateListeners = [];

      // Mock AppState.currentState
      Object.defineProperty(AppState, 'currentState', {
        value: 'active',
        writable: true,
        configurable: true,
      });

      // Mock AppState.addEventListener
      jest.spyOn(AppState, 'addEventListener').mockImplementation((event, listener) => {
        if (event === 'change') {
          appStateListeners.push(listener);
        }
        return {
          remove: jest.fn(() => {
            const index = appStateListeners.indexOf(listener);
            if (index > -1) {
              appStateListeners.splice(index, 1);
            }
          }),
        };
      });
    });

    it('should update locale when app becomes active and locale changed', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      expect(result.current.currentLocale).toBe('en-US');

      // Simulate app going to background
      act(() => {
        appStateListeners.forEach((listener) => listener('background'));
      });

      // Change locale
      (ExpoLocalization.getLocales as jest.Mock).mockReturnValue([mockFrenchLocale]);

      // Simulate the app becoming active
      act(() => {
        appStateListeners.forEach((listener) => listener('active'));
      });

      expect(result.current.currentLocale).toBe('fr-CA');
      expect(result.current.currencyCode).toBe('CAD');
    });

    it('should not update locale when app becomes active but locale unchanged', () => {
      const { result } = renderHook(() => useLocaleFormatter());
      const getLocalesSpy = jest.spyOn(ExpoLocalization, 'getLocales');

      expect(result.current.currentLocale).toBe('en-US');

      // Clear spy calls from the initial render
      getLocalesSpy.mockClear();

      // Simulate app going to background
      act(() => {
        appStateListeners.forEach((listener) => listener('background'));
      });

      // Simulates app becoming active (locale unchanged)
      act(() => {
        appStateListeners.forEach((listener) => listener('active'));
      });

      // getLocales should be called to check
      expect(getLocalesSpy).toHaveBeenCalled();
      // But locale should remain the same
      expect(result.current.currentLocale).toBe('en-US');
    });

    it('should not update locale when app state changes to inactive', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      expect(result.current.currentLocale).toBe('en-US');

      // Simulate app going to inactive
      act(() => {
        appStateListeners.forEach((listener) => listener('inactive'));
      });

      expect(result.current.currentLocale).toBe('en-US');
    });

    it('should remove listener on unmount', () => {
      const { unmount } = renderHook(() => useLocaleFormatter());
      const removeSpy = jest.fn();

      // Get the remove function
      const subscription = (AppState.addEventListener as jest.Mock).mock.results[0].value;
      subscription.remove = removeSpy;

      unmount();

      expect(removeSpy).toHaveBeenCalled();
    });
  });

  describe('memoization', () => {
    it('should memoize formatter functions when locale unchanged', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      const firstFormatCurrency = result.current.formatCurrency;
      const firstFormatDate = result.current.formatDate;
      const firstFormatPercentage = result.current.formatPercentage;

      // Call the formatters - they should remain the same
      result.current.formatCurrency(100);

      expect(result.current.formatCurrency).toBe(firstFormatCurrency);
      expect(result.current.formatDate).toBe(firstFormatDate);
      expect(result.current.formatPercentage).toBe(firstFormatPercentage);
    });

    it('should update formatter functions when locale changes', () => {
      let appStateListeners: ((state: AppStateStatus) => void)[] = [];

      // Mock AppState.currentState
      Object.defineProperty(AppState, 'currentState', {
        value: 'active',
        writable: true,
        configurable: true,
      });

      jest.spyOn(AppState, 'addEventListener').mockImplementation((event, listener) => {
        if (event === 'change') {
          appStateListeners.push(listener);
        }
        return {
          remove: jest.fn(() => {
            const index = appStateListeners.indexOf(listener);
            if (index > -1) {
              appStateListeners.splice(index, 1);
            }
          }),
        };
      });

      const { result } = renderHook(() => useLocaleFormatter());

      const firstFormatCurrency = result.current.formatCurrency;

      // Simulate app going to background
      act(() => {
        appStateListeners.forEach((listener) => listener('background'));
      });

      // Change locale
      (ExpoLocalization.getLocales as jest.Mock).mockReturnValue([mockFrenchLocale]);

      // Simulate the app becoming active
      act(() => {
        appStateListeners.forEach((listener) => listener('active'));
      });

      expect(result.current.formatCurrency).not.toBe(firstFormatCurrency);
    });
  });
});
