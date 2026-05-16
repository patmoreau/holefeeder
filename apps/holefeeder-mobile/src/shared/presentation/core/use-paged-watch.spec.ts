import { act, renderHook, waitFor } from '@testing-library/react-native';
import { FlowsRepositoryInMemory } from '@/flows/core/flows/__tests__/flows-repository-in-memory';
import { aTransaction, toTransaction } from '@/flows/core/flows/__tests__/transaction-for-test';
import { Transaction } from '@/flows/core/flows/transaction';
import { anId } from '@/shared/__tests__/string-for-test';
import { DateOnly } from '@/shared/core/date-only';
import { usePagedWatch, WatchCountFn, WatchPageFn } from './use-paged-watch';

const PAGE_SIZE = 3;
const MAX_PAGES = 1;

describe('usePagedWatch', () => {
  const accountId = anId();
  const transaction0 = toTransaction(aTransaction({ accountId: accountId, date: DateOnly.valid('2025-01-10') }));
  const transaction1 = toTransaction(aTransaction({ accountId: accountId, date: DateOnly.valid('2025-01-09') }));
  const transaction2 = toTransaction(aTransaction({ accountId: accountId, date: DateOnly.valid('2025-01-08') }));
  const transaction3 = toTransaction(aTransaction({ accountId: accountId, date: DateOnly.valid('2025-01-07') }));
  const transaction4 = toTransaction(aTransaction({ accountId: accountId, date: DateOnly.valid('2025-01-06') }));
  let flowRepository: FlowsRepositoryInMemory;
  let watchFn: WatchPageFn<Transaction>;
  let watchCountFn: WatchCountFn;

  const createHook = async (watchFn: WatchPageFn<Transaction>, watchCountFn: WatchCountFn, maxPages?: number) =>
    await waitFor(() =>
      renderHook(() => usePagedWatch<Transaction>(watchFn, watchCountFn, { pageSize: PAGE_SIZE, maxPages: maxPages ?? MAX_PAGES }))
    );

  beforeEach(() => {
    flowRepository = FlowsRepositoryInMemory();
    watchFn = (onData, limit, offset) =>
      flowRepository.watchTransactions(
        (result) => {
          if (result.isSuccess) onData(result.value);
        },
        accountId,
        limit,
        offset
      );

    watchCountFn = (onCount) =>
      flowRepository.watchTransactionCount((result) => {
        if (result.isSuccess) onCount(result.value);
      }, accountId);
  });

  describe('initial state', () => {
    it('returns loading before any data arrives', async () => {
      flowRepository.isLoading();
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.data).toBeLoading();
      expect(result.current.totalCount).toBe(0);
      expect(result.current.loading).toBe(false);
      expect(result.current.hasNextPage).toBe(false);
      expect(result.current.hasPreviousPage).toBe(false);
    });
  });

  describe('data', () => {
    it('mounts with correct paged data', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2, transaction3, transaction4);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);
    });

    it('remains unchanged when data added outside paging', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);

      await act(async () => {
        flowRepository.addTransactions(transaction3, transaction4);
      });

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);
    });

    it('updates data when data added inside paging', async () => {
      flowRepository.addTransactions(transaction0, transaction1);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1]);

      await act(async () => {
        flowRepository.addTransactions(transaction2);
      });

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);
    });

    it('merges all loaded pages into data', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2, transaction3, transaction4);
      const { result } = await createHook(watchFn, watchCountFn, 2);

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);

      await act(async () => result.current.loadNext());
      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2, transaction3, transaction4]);
    });

    it('trims the oldest page when maxPages is exceeded', async () => {
      flowRepository.addTransactions(
        transaction0,
        transaction1,
        transaction2,
        transaction3,
        transaction4,
        transaction0,
        transaction1,
        transaction2,
        transaction3,
        transaction4
      );
      const { result } = await createHook(watchFn, watchCountFn, 2);

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);

      await act(async () => result.current.loadNext());
      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2, transaction3, transaction4, transaction0]);

      await act(async () => result.current.loadNext());
      expect(result.current.data).toBeSuccessWithValue([transaction3, transaction4, transaction0, transaction1, transaction2, transaction3]);
    });
  });

  describe('totalCount', () => {
    it('subscribes to watchCountFn on mount', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2, transaction3, transaction4);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.totalCount).toBe(5);
    });

    it('updates when count changes', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.totalCount).toBe(3);

      await act(async () => {
        flowRepository.addTransactions(transaction3, transaction4);
      });

      expect(result.current.totalCount).toBe(5);
    });
  });

  describe('hasNextPage', () => {
    it('is false when all items fit in the loaded pages', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.hasNextPage).toBe(false);
    });

    it('is true when there are more items beyond the max loaded page', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2, transaction3, transaction4);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.hasNextPage).toBe(true);
    });

    it('updates reactively when count changes', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.hasNextPage).toBe(false);

      await act(async () => {
        flowRepository.addTransactions(transaction3);
      });

      expect(result.current.hasNextPage).toBe(true);
    });
  });

  describe('hasPreviousPage', () => {
    it('is false when all items fit in the loaded pages', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2);
      const { result } = await createHook(watchFn, watchCountFn);

      expect(result.current.hasPreviousPage).toBe(false);
    });

    it('is true when there are more items before the latest loaded page', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2, transaction3, transaction4);
      const { result } = await createHook(watchFn, watchCountFn);

      await act(async () => result.current.loadNext());
      expect(result.current.hasPreviousPage).toBe(true);
    });
  });

  describe('loadNext', () => {
    it('loads next page', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2, transaction3, transaction4);
      const { result } = await createHook(watchFn, watchCountFn);

      act(() => result.current.loadNext());

      expect(result.current.data).toBeSuccessWithValue([transaction3, transaction4]);
    });

    it('does nothing when hasNextPage is false', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2);
      const { result } = await createHook(watchFn, watchCountFn);

      act(() => result.current.loadNext());

      expect(result.current.hasNextPage).toBe(false);
      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);
    });
  });

  describe('loadPrevious', () => {
    it('loads previous page', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2, transaction3, transaction4);
      const { result } = await createHook(watchFn, watchCountFn);

      act(() => result.current.loadNext());
      expect(result.current.data).toBeSuccessWithValue([transaction3, transaction4]);
      act(() => result.current.loadPrevious());

      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);
    });

    it('does nothing when hasPreviousPage is false', async () => {
      flowRepository.addTransactions(transaction0, transaction1, transaction2);
      const { result } = await createHook(watchFn, watchCountFn);

      act(() => result.current.loadPrevious());

      expect(result.current.hasPreviousPage).toBe(false);
      expect(result.current.data).toBeSuccessWithValue([transaction0, transaction1, transaction2]);
    });
  });
});
