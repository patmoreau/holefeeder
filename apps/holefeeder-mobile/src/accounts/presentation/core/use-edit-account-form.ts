import { Result } from '@holefeeder/core';
import { UpdateAccountCommand } from '@/accounts/core/update/update-account-command';
import { UpdateAccountUseCase } from '@/accounts/core/update/update-account-use-case';
import { EditAccountFormData } from '@/accounts/presentation/core/edit-account-form-data';
import { createFormDataContext, ValidationFunction } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const EditAccountFormError = {
  nameRequired: 'nameRequired',
} as const;

export type EditAccountFormError = (typeof EditAccountFormError)[keyof typeof EditAccountFormError];

export const validateEditAccountForm: ValidationFunction<EditAccountFormData, EditAccountFormError> = (formData) => {
  const errors: Partial<Record<keyof EditAccountFormData, EditAccountFormError>> = {};
  if (!formData.name.trim()) {
    errors.name = EditAccountFormError.nameRequired;
  }
  return errors;
};

const saveAccount = async (repositories: RepositoriesState, formData: EditAccountFormData): Promise<Result<unknown>> => {
  const commandResult = UpdateAccountCommand.create({
    id: formData.id,
    type: formData.type,
    name: formData.name,
    openBalance: formData.openBalance,
    openDate: formData.openDate,
    description: formData.description,
    favorite: formData.favorite,
    inactive: formData.inactive,
  });
  if (commandResult.isFailure) return commandResult;

  const useCase = UpdateAccountUseCase(repositories.accountRepository);

  return await useCase.execute(commandResult.value);
};

export const { FormDataProvider: EditAccountFormProvider, useFormDataContext: useEditAccountForm } = createFormDataContext<
  EditAccountFormData,
  EditAccountFormError
>('EditAccount', saveAccount);
