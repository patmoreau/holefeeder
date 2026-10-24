import { useApiQuery } from '../../shared/api/use-api-query';
import { Account } from '../domain/account';

export const useAccounts = () => useApiQuery('/api/v2/accounts', (raw: Record<string, unknown>[]) => raw.map(Account.valid));
