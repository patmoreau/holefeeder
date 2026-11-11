import { useApiQuery } from '../../shared/api/use-api-query';
import { DashboardSummary } from '../domain/dashboard-summary';

export const useDashboardSummary = () => useApiQuery('/api/v2/dashboard/summary', DashboardSummary.valid);
