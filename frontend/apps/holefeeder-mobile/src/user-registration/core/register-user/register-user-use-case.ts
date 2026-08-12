import { Result } from '@holefeeder/shared/core';
import { ApiErrors } from '@/shared/api/api-errors';
import { UsersApi } from '@/shared/api/users-api';

export const RegisterUserUseCase = (usersApi: UsersApi) => {
  const execute = async (): Promise<Result<void>> => {
    const result = await usersApi.register();

    if (result.isSuccess) {
      return Result.success();
    }

    // The endpoint answers 400 for a caller who already has an account. Onboarding
    // retries, so the second attempt after a succeeded-but-lost response must not
    // strand the user on an error screen: the goal is "registered", and they are.
    return result.errors.includes(ApiErrors.badRequest) ? Result.success() : result;
  };

  return {
    execute: execute,
  };
};
