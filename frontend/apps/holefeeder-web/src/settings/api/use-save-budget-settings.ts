import type { Settings } from '@holefeeder/shared/core';
import { useApiCommand } from '@/shared/api/use-api-command';

export const useSaveBudgetSettings = () => {
  const command = useApiCommand();

  return (settings: Settings) =>
    command('/api/v2/settings/save-settings', {
      effectiveDate: settings.effectiveDate,
      intervalType: settings.intervalType,
      frequency: settings.frequency,
    });
};
