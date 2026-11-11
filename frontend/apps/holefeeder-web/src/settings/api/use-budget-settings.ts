import { useApiQuery } from '@/shared/api/use-api-query';
import { BudgetSettings } from '../domain/budget-settings';

export const useBudgetSettings = () => useApiQuery('/api/v2/settings', BudgetSettings.valid);
