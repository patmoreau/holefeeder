import { act, renderHook } from '@testing-library/react-native';
import * as ExpoLocalization from 'expo-localization';
import { AppState, AppStateStatus } from 'react-native';
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
  });

  describe('locale change detection', () => {
    let appStateListeners: ((state: AppStateStatus) => void)[] = [];

    beforeEach(() => {
      appStateListeners = [];

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
    });

    it('should update locale when app becomes active and locale changed', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      expect(result.current.currentLocale).toBe('en-US');

      act(() => {
        appStateListeners.forEach((listener) => listener('background'));
      });

      (ExpoLocalization.getLocales as jest.Mock).mockReturnValue([mockFrenchLocale]);

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

      getLocalesSpy.mockClear();

      act(() => {
        appStateListeners.forEach((listener) => listener('background'));
      });

      act(() => {
        appStateListeners.forEach((listener) => listener('active'));
      });

      expect(getLocalesSpy).toHaveBeenCalled();
      expect(result.current.currentLocale).toBe('en-US');
    });

    it('should not update locale when app state changes to inactive', () => {
      const { result } = renderHook(() => useLocaleFormatter());

      expect(result.current.currentLocale).toBe('en-US');

      act(() => {
        appStateListeners.forEach((listener) => listener('inactive'));
      });

      expect(result.current.currentLocale).toBe('en-US');
    });

    it('should remove listener on unmount', () => {
      const { unmount } = renderHook(() => useLocaleFormatter());
      const removeSpy = jest.fn();

      const subscription = (AppState.addEventListener as jest.Mock).mock.results[0].value;
      subscription.remove = removeSpy;

      unmount();

      expect(removeSpy).toHaveBeenCalled();
    });
  });
});
