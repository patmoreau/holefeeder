import { Id, Result, StoreItem, StoreItemsRepository } from '@holefeeder/shared/core';
import { SaveSettingsCommand } from '@/settings/core/save-settings/save-settings-command';
import { Settings, SETTINGS_CODE } from '@/shared/core/settings';

export const SaveSettingsUseCase = (repository: StoreItemsRepository) => {
  const execute = async (command: SaveSettingsCommand): Promise<Result<void>> => {
    const settingsResult = Settings.create(command);
    if (!settingsResult.isSuccess) {
      return settingsResult;
    }

    const settings = settingsResult.value;
    let storeItem = await repository.getByCode(SETTINGS_CODE);
    if (!storeItem.isSuccess) {
      storeItem = StoreItem.create({
        id: Id.newId(),
        code: SETTINGS_CODE,
        data: JSON.stringify(settings),
      });
    } else {
      storeItem = StoreItem.create({
        id: storeItem.value.id,
        code: SETTINGS_CODE,
        data: JSON.stringify(settings),
      });
    }

    if (storeItem.isSuccess) {
      return await repository.save(storeItem.value);
    }

    return storeItem;
  };

  return {
    execute: execute,
  };
};
