import { useApiQuery } from '../../shared/api/use-api-query';
import { DashboardUpcomingFlow } from '../domain/dashboard-upcoming-flow';

export const useUpcomingFlows = () => useApiQuery('/api/v2/dashboard/upcoming', DashboardUpcomingFlow.validList);
