import { ApiClient } from '@holefeeder/shared/api';
import { AuthenticationState, Result } from '@holefeeder/shared/core';
import { ApiConfig } from '@/shared/api/api-config';

export type CurrentUser = {
  id: string;
};

export type UsersApi = {
  getMe: () => Promise<Result<CurrentUser>>;
  register: () => Promise<Result<CurrentUser>>;
};

export const usersApi = (authenticationState: AuthenticationState, apiConfig: ApiConfig): UsersApi => {
  const api = ApiClient(authenticationState, apiConfig);

  const getMe = (): Promise<Result<CurrentUser>> => api.get<CurrentUser>('/api/v2/users/me');

  const register = (): Promise<Result<CurrentUser>> => api.post<undefined, CurrentUser>('/api/v2/users/register');

  return {
    getMe: getMe,
    register: register,
  };
};
