import { ApiClient } from '@holefeeder/shared/api';
import { useAuthentication } from '@/shared/auth/authentication-context';
import { apiConfig } from './api-config';

export const useApiCommand = () => {
  const auth = useAuthentication();

  return <TInput, TOutput = void>(endpoint: string, body: TInput) => ApiClient(auth, apiConfig).post<TInput, TOutput>(endpoint, body);
};
