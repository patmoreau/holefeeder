import { AsyncResult, Logger, Result } from '@holefeeder/shared/core';
import { useCallback, useRef, useState } from 'react';
import { ApiConfig } from '@/shared/api/api-config';
import { usersApi } from '@/shared/api/users-api';
import { useAuth } from '@/shared/auth/core/use-auth';
import { RegisterUserUseCase } from '@/user-registration/core/register-user/register-user-use-case';

const logger = Logger.create('use-register-user');

export type RegisterUserState = {
  registration: AsyncResult<void>;
  register: () => void;
};

export const useRegisterUser = (apiConfig: ApiConfig): RegisterUserState => {
  const authenticationState = useAuth();

  // Same reason as the status hook: the provider hands back a fresh object every
  // render, and register must not be a new function on each one.
  const authenticationRef = useRef(authenticationState);
  authenticationRef.current = authenticationState;

  const [registration, setRegistration] = useState<AsyncResult<void>>(Result.loading());

  const register = useCallback(() => {
    setRegistration(Result.loading());

    const useCase = RegisterUserUseCase(usersApi(authenticationRef.current, apiConfig));
    useCase.execute().then((result) => {
      if (result.isFailure) {
        logger.error('Could not register the caller', result.errors);
      }
      setRegistration(result);
    });
  }, [apiConfig]);

  return {
    registration: registration,
    register: register,
  };
};
