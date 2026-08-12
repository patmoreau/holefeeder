import { AsyncResult, Logger, Result } from '@holefeeder/shared/core';
import { useEffect, useRef, useState } from 'react';
import { ApiConfig } from '@/shared/api/api-config';
import { usersApi } from '@/shared/api/users-api';
import { useAuth } from '@/shared/auth/core/use-auth';
import { CheckRegistrationUseCase } from '@/user-registration/core/check-registration/check-registration-use-case';
import { RegistrationStatus } from '@/user-registration/core/registration-status';

const logger = Logger.create('use-registration-status');

export type RegistrationState = {
  status: AsyncResult<RegistrationStatus>;
  recheck: () => void;
};

export const useRegistrationStatus = (apiConfig: ApiConfig): RegistrationState => {
  const authenticationState = useAuth();
  const userId = authenticationState.user?.sub;

  // The authentication provider builds a fresh state object on every render. Holding
  // it in a ref keeps the check keyed on who the caller is, not on React's render
  // cadence — depending on the object directly would re-hit the API each render.
  const authenticationRef = useRef(authenticationState);
  authenticationRef.current = authenticationState;

  const [status, setStatus] = useState<AsyncResult<RegistrationStatus>>(Result.loading());
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    if (!userId) {
      setStatus(Result.loading());
      return;
    }

    let cancelled = false;
    setStatus(Result.loading());

    const useCase = CheckRegistrationUseCase(usersApi(authenticationRef.current, apiConfig));
    useCase.execute().then((result) => {
      if (cancelled) {
        return;
      }
      if (result.isFailure) {
        logger.error('Could not determine registration status', result.errors);
      }
      setStatus(result);
    });

    return () => {
      cancelled = true;
    };
  }, [apiConfig, attempt, userId]);

  const recheck = () => setAttempt((previous) => previous + 1);

  return {
    status: status,
    recheck: recheck,
  };
};
