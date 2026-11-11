import { useApiQuery } from '@/shared/api/use-api-query';
import { TransactionDetail } from '../domain/transaction-detail';

export const useTransactionDetail = (id: string) => useApiQuery(`/api/v2/transactions/${id}`, TransactionDetail.valid);
