import { useApiQuery } from '@/shared/api/use-api-query';
import { NewTransactionOptions } from '../domain/new-transaction-options';

export const useNewTransactionOptions = () => useApiQuery('/api/v2/transactions/new-transaction-options', NewTransactionOptions.valid);
