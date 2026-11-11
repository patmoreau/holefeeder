import { useApiQuery } from '../../shared/api/use-api-query';
import { DashboardTransaction } from '../domain/dashboard-transaction';

export const useRecentTransactions = () => useApiQuery('/api/v2/dashboard/transactions', DashboardTransaction.validList);
