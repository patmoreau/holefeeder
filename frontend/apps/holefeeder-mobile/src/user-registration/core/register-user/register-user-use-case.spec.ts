import { Result } from '@holefeeder/shared/core';
import { ApiErrors } from '@/shared/api/api-errors';
import { aUsersApi } from '@/user-registration/core/__tests__/users-api-for-test';
import { RegisterUserUseCase } from '@/user-registration/core/register-user/register-user-use-case';

describe('RegisterUserUseCase', () => {
  it('should register the caller', async () => {
    const useCase = RegisterUserUseCase(aUsersApi());

    const result = await useCase.execute();

    expect(result).toBeSuccessWithValue(undefined);
  });

  it('should treat an already registered caller as success', async () => {
    const usersApi = aUsersApi({ register: () => Promise.resolve(Result.failure([ApiErrors.badRequest])) });
    const useCase = RegisterUserUseCase(usersApi);

    const result = await useCase.execute();

    expect(result).toBeSuccessWithValue(undefined);
  });

  it('should fail when the server errors', async () => {
    const usersApi = aUsersApi({ register: () => Promise.resolve(Result.failure([ApiErrors.serverError])) });
    const useCase = RegisterUserUseCase(usersApi);

    const result = await useCase.execute();

    expect(result).toBeFailureWithErrors([ApiErrors.serverError]);
  });

  it('should fail when the token is rejected', async () => {
    const usersApi = aUsersApi({ register: () => Promise.resolve(Result.failure([ApiErrors.unauthorized])) });
    const useCase = RegisterUserUseCase(usersApi);

    const result = await useCase.execute();

    expect(result).toBeFailureWithErrors([ApiErrors.unauthorized]);
  });
});
