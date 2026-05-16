import { SaveSettingsCommand } from '@/settings/core/save-settings/save-settings-command';
import { SaveSettingsUseCase } from '@/settings/core/save-settings/save-settings-use-case';
import { SettingsFormData } from '@/settings/core/settings-form-data';
import { Result } from '@/shared/core/result';
import { createFormDataContext, ValidationFunction } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const SettingsFormError = {
  effectiveDateRequired: 'effectiveDateRequired',
  frequencyRequired: 'frequencyRequired',
} as const;

export type SettingsFormError = (typeof SettingsFormError)[keyof typeof SettingsFormError];

export const validateSettingsForm: ValidationFunction<SettingsFormData, SettingsFormError> = (formData) => {
  const errors: Partial<Record<keyof SettingsFormData, SettingsFormError>> = {};

  if (formData.effectiveDate === '') {
    errors.effectiveDate = SettingsFormError.effectiveDateRequired;
  }

  if (formData.frequency <= 0) {
    errors.frequency = SettingsFormError.frequencyRequired;
  }

  return errors;
};

const save = async (repositories: RepositoriesState, formData: SettingsFormData): Promise<Result<unknown>> => {
  const useCase = SaveSettingsUseCase(repositories.storeItemRepository);
  const command = SaveSettingsCommand.create(formData);
  if (command.isFailure || command.isLoading) return command;
  return useCase.execute(command.value);
};

export const { FormDataProvider: SettingsFormProvider, useFormDataContext: useSettingsForm } = createFormDataContext<
  SettingsFormData,
  SettingsFormError
>('Settings', save);
