import { Result } from '@holefeeder/shared/core';
import { ApiErrors } from '@/shared/api/api-errors';
import { UsersApi } from '@/shared/api/users-api';
import { RegistrationStatus, RegistrationStatuses } from '@/user-registration/core/registration-status';

export const CheckRegistrationUseCase = (usersApi: UsersApi) => {
  const execute = async (): Promise<Result<RegistrationStatus>> => {
    const result = await usersApi.getMe();

    if (result.isSuccess) {
      return Result.success(RegistrationStatuses.registered);
    }

    // Only a 404 means "we asked and this caller has no account". Anything else —
    // the server is down, the token was rejected — must stay a failure, or the app
    // would send an existing user through onboarding.
    return result.errors.includes(ApiErrors.notFound) ? Result.success(RegistrationStatuses.notRegistered) : result;
  };

  return {
    execute: execute,
  };
};
