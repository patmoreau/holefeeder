import { waitFor } from '@testing-library/react-native';
import { aSettings } from '@/settings/core/__tests__/settings-for-test';
import { DefaultSettings, Settings, SETTINGS_CODE } from '@/settings/core/settings';
import { aStoreItem } from '@/shared/__tests__/store-item-for-test';
import { StoreItemsRepositoryInMemory } from '@/shared/__tests__/store-items-repository-for-test';
import { type AsyncResult } from '@/shared/core/result';
import { GetSettingsUseCase } from './get-settings-use-case';

describe('GetSettingsUseCase', () => {
  let repository: StoreItemsRepositoryInMemory;
  let useCase: ReturnType<typeof GetSettingsUseCase>;

  beforeEach(() => {
    repository = StoreItemsRepositoryInMemory();
    useCase = GetSettingsUseCase(repository);
  });

  describe('watchForCode', () => {
    it('returns settings', async () => {
      const settings = aSettings();
      const storeItem = aStoreItem({ code: SETTINGS_CODE, data: JSON.stringify(settings) });
      repository.add(storeItem);

      let result: AsyncResult<Settings> | undefined;
      const unsubscribe = useCase.watchForCode((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue(settings);

      unsubscribe();
    });

    it('returns default settings when code not found', async () => {
      let result: AsyncResult<Settings> | undefined;
      const unsubscribe = useCase.watchForCode((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeSuccessWithValue(DefaultSettings);

      unsubscribe();
    });

    it('returns failure when repository fails', async () => {
      const storeItem = aStoreItem({ code: SETTINGS_CODE });
      repository.add(storeItem);
      repository.isFailing(['error']);

      let result: AsyncResult<Settings> | undefined;
      const unsubscribe = useCase.watchForCode((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeFailureWithErrors(['error']);

      unsubscribe();
    });

    it('returns loading when repository is loading', async () => {
      const storeItem = aStoreItem({ code: SETTINGS_CODE });
      repository.add(storeItem);
      repository.isLoading();

      let result: AsyncResult<Settings> | undefined;
      const unsubscribe = useCase.watchForCode((data) => {
        result = data;
      });

      await waitFor(() => expect(result).toBeDefined());

      expect(result).toBeLoading();

      unsubscribe();
    });
  });
});
