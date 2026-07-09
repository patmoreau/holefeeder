import { useQuery } from '@tanstack/react-query';
import { ApiClient } from '../../shared/api/api-client';
import { useAuthentication } from '../../shared/auth/authentication-provider';
import { Account } from '../domain/account';

export const useAccounts = () => {
  const auth = useAuthentication();

  return useQuery({
    queryKey: ['accounts'],
    queryFn: async () => {
      const client = ApiClient(auth, {
        url: import.meta.env.VITE_API_BASE_URL as string,
        timeout: 10_000,
      });
      const result = await client.get<Record<string, unknown>[]>('/api/v2/accounts');
      if (result.isFailure) throw new Error(result.errors.join(', '));
      return result.value.map(Account.valid);
    },
    enabled: !!auth.user && !auth.isLoading,
  });
};
