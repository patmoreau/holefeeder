import { Result } from '@holefeeder/shared/core';
import { renderHook } from '@testing-library/react-native';
import { ErrorKey } from '@/shared/core/error-key';
import { useMultipleWatches, withDefault } from '@/shared/presentation/core/use-multiple-watches';

describe('useMultipleWatches', () => {
  describe('data', () => {
    it('maps each successful watch to its value', async () => {
      const { result } = await renderHook(() =>
        useMultipleWatches({
          a: () => Result.success('alpha'),
          b: () => Result.success(42),
        })
      );

      expect(result.current.data).toEqual({ a: 'alpha', b: 42 });
    });

    it('is undefined for a loading watch without a default', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: () => Result.loading() }));

      expect(result.current.data.a).toBeUndefined();
    });

    it('uses the default value for a loading watch wrapped with withDefault', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: withDefault(() => Result.loading<string>(), 'fallback') }));

      expect(result.current.data.a).toBe('fallback');
    });

    it('is undefined for a failing watch without a default', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: () => Result.failure(['boom']) }));

      expect(result.current.data.a).toBeUndefined();
    });
  });

  describe('isLoading', () => {
    it('is true when any watch is loading', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: () => Result.success('x'), b: () => Result.loading() }));

      expect(result.current.isLoading).toBe(true);
    });

    it('is false when all watches have resolved', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: () => Result.success('x'), b: () => Result.success('y') }));

      expect(result.current.isLoading).toBe(false);
    });
  });

  describe('errors', () => {
    it('shows the error when any watch fails', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: () => Result.failure(['boom']) }));

      expect(result.current.errors.showError).toBe(true);
      expect(result.current.errors.error).toBe(ErrorKey.saveFailed);
    });

    it('does not show an error when all watches succeed', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: () => Result.success('x') }));

      expect(result.current.errors.showError).toBe(false);
    });
  });

  describe('results', () => {
    it('exposes the raw AsyncResult value for each watch', async () => {
      const { result } = await renderHook(() => useMultipleWatches({ a: () => Result.success('x'), b: () => Result.loading() }));

      expect(result.current.results.a.isSuccess).toBe(true);
      expect(result.current.results.b.isLoading).toBe(true);
    });
  });
});
