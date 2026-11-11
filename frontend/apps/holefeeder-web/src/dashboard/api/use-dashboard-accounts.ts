import { useApiQuery } from '../../shared/api/use-api-query';
import { DashboardAccount } from '../domain/dashboard-account';

export const useDashboardAccounts = () => useApiQuery('/api/v2/dashboard/accounts', DashboardAccount.validList);
