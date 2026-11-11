import { useApiCommand } from '@/shared/api/use-api-command';
import type { BudgetSettings } from '../domain/budget-settings';

export const useSaveBudgetSettings = () => {
  const command = useApiCommand();

  return (settings: BudgetSettings) =>
    command('/api/v2/settings/save-settings', {
      effectiveDate: settings.effectiveDate,
      intervalType: settings.intervalType,
      frequency: settings.frequency,
    });
};
