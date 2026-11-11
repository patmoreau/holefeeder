import { useAuthentication } from '@/shared/auth/authentication-context';
import { ApiClient } from './api-client';
import { apiConfig } from './api-config';

export const useDeleteTransaction = () => {
  const auth = useAuthentication();

  return (id: string) => ApiClient(auth, apiConfig).delete(`/api/v2/transactions/${id}`);
};
