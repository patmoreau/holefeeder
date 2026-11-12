import { ApiClient } from '@/shared/api/api-client';
import { apiConfig } from '@/shared/api/api-config';
import { useAuthentication } from '@/shared/auth/authentication-context';

export const useDeleteCategory = () => {
  const auth = useAuthentication();

  return (id: string) => ApiClient(auth, apiConfig).delete(`/api/v2/categories/${id}`);
};
