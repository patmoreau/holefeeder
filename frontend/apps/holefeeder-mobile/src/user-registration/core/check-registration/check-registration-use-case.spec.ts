import { Result } from '@holefeeder/shared/core';
import { ApiErrors } from '@/shared/api/api-errors';
import { aUsersApi } from '@/user-registration/core/__tests__/users-api-for-test';
import { CheckRegistrationUseCase } from '@/user-registration/core/check-registration/check-registration-use-case';
import { RegistrationStatuses } from '@/user-registration/core/registration-status';

describe('CheckRegistrationUseCase', () => {
  it('should report a known caller as registered', async () => {
    const useCase = CheckRegistrationUseCase(aUsersApi());

    const result = await useCase.execute();

    expect(result).toBeSuccessWithValue(RegistrationStatuses.registered);
  });

  it('should report a caller the api does not know as not registered', async () => {
    const usersApi = aUsersApi({ getMe: () => Promise.resolve(Result.failure([ApiErrors.notFound])) });
    const useCase = CheckRegistrationUseCase(usersApi);

    const result = await useCase.execute();

    expect(result).toBeSuccessWithValue(RegistrationStatuses.notRegistered);
  });

  it('should fail rather than assume a caller is unregistered when the server errors', async () => {
    const usersApi = aUsersApi({ getMe: () => Promise.resolve(Result.failure([ApiErrors.serverError])) });
    const useCase = CheckRegistrationUseCase(usersApi);

    const result = await useCase.execute();

    expect(result).toBeFailureWithErrors([ApiErrors.serverError]);
  });

  it('should fail when the token is rejected', async () => {
    const usersApi = aUsersApi({ getMe: () => Promise.resolve(Result.failure([ApiErrors.unauthorized])) });
    const useCase = CheckRegistrationUseCase(usersApi);

    const result = await useCase.execute();

    expect(result).toBeFailureWithErrors([ApiErrors.unauthorized]);
  });
});
