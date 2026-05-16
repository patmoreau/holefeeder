import { type AsyncResult, Result, StoreItem, StoreItemsRepository, StoreItemsRepositoryErrors } from '@holefeeder/shared/core';
import { DefaultSettings, Settings, SETTINGS_CODE } from '@/settings/core/settings';

export const GetSettingsUseCase = (repository: StoreItemsRepository) => {
  const watchForCode = (onDataChange: (result: AsyncResult<Settings>) => void) =>
    repository.watchForCode(SETTINGS_CODE, (storeItemResult: AsyncResult<StoreItem>) => {
      if (storeItemResult.isLoading) {
        onDataChange(storeItemResult as AsyncResult<Settings>);
        return;
      }

      if (storeItemResult.isFailure) {
        if (storeItemResult.errors.includes(StoreItemsRepositoryErrors.storeItemNotFound)) {
          onDataChange(Result.success(DefaultSettings));
        } else {
          onDataChange(storeItemResult as AsyncResult<Settings>);
        }
        return;
      }

      const storeItem = storeItemResult.value;
      const settingsData = JSON.parse(storeItem.data);
      const settingsResult = Settings.create(settingsData);
      onDataChange(settingsResult);
    });

  return {
    watchForCode: watchForCode,
  };
};
