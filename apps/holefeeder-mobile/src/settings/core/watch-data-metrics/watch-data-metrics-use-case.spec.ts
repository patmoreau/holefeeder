import { waitFor } from '@testing-library/react-native';
import { SettingsRepositoryInMemory } from '@/settings/core/__tests__/settings-repository-for-test';
import { DataMetrics } from '@/settings/core/data-metrics';
import { WatchDataMetricsUseCase } from '@/settings/core/watch-data-metrics/watch-data-metrics-use-case';
import { type AsyncResult } from '@/shared/core/result';

describe('WatchDataMetricsUseCase', () => {
  let settingsRepo: SettingsRepositoryInMemory;
  let useCase: ReturnType<typeof WatchDataMetricsUseCase>;

  beforeEach(() => {
    settingsRepo = SettingsRepositoryInMemory();
    useCase = WatchDataMetricsUseCase(settingsRepo);
  });

  describe('watch', () => {
    it('returns sync info when returns data', async () => {
      settingsRepo.add({ accounts: 1, cashflows: 2, categories: 3, storeItems: 4, transactions: 5, outstandingTransactions: 6 });

      let result: AsyncResult<DataMetrics> | undefined;
      const unsubscribe = useCase.watch((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue({
        accounts: 1,
        cashflows: 2,
        categories: 3,
        storeItems: 4,
        transactions: 5,
        outstandingTransactions: 6,
      });

      unsubscribe();
    });

    it('should return failure when settings repository fails', async () => {
      settingsRepo.isFailing(['error']);

      let result: AsyncResult<DataMetrics> | undefined;
      const unsubscribe = useCase.watch((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeFailureWithErrors(['error']);

      unsubscribe();
    });

    it('should return loading when settings repository is loading', async () => {
      settingsRepo.isLoading();

      let result: AsyncResult<DataMetrics> | undefined;
      const unsubscribe = useCase.watch((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeLoading();

      unsubscribe();
    });
  });
});
