import { useEffect, useMemo, useState } from 'react';
import { GetSettingsUseCase } from '@/settings/core/get-settings/get-settings-use-case';
import type { Settings } from '@/settings/core/settings';
import { type AsyncResult, Result } from '@/shared/core/result';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export const useSettings = (): AsyncResult<Settings> => {
  const { storeItemRepository } = useRepositories();
  const [settings, setSettings] = useState<AsyncResult<Settings>>(Result.loading());

  const useCase = useMemo(() => GetSettingsUseCase(storeItemRepository), [storeItemRepository]);

  useEffect(() => {
    const unsubscribe = useCase.watchForCode(setSettings);
    return () => unsubscribe();
  }, [useCase]);

  const stableSettings = useMemo(() => {
    if (settings.isSuccess) {
      return settings;
    }
    return settings;
  }, [settings]);

  return stableSettings;
};
