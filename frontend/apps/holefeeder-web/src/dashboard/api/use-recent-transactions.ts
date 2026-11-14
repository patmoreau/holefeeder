import { TransactionSummary } from '@/shared/domain/transaction-summary';
import { useApiQuery } from '../../shared/api/use-api-query';

export const useRecentTransactions = () => useApiQuery('/api/v2/dashboard/transactions', TransactionSummary.validList);
