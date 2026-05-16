import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { ModifyFlowUseCase } from '@/flows/core/flows/modify/modify-flow-use-case';
import { FlowFormData } from '@/flows/presentation/flow/core/flow-form-data';
import { Money } from '@/shared/core/money';
import { Result } from '@/shared/core/result';
import { createFormDataContext, ValidationFunction } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const FlowFormError = {
  amountRequired: 'amountRequired',
  accountRequired: 'accountRequired',
  categoryRequired: 'categoryRequired',
} as const;

export type FlowFormError = (typeof FlowFormError)[keyof typeof FlowFormError];

export const validateFormForm: ValidationFunction<FlowFormData, FlowFormError> = (formData) => {
  const errors: Partial<Record<keyof FlowFormData, FlowFormError>> = {};

  if (Money.create(formData.amount).isFailure) {
    errors.amount = FlowFormError.amountRequired;
  }

  if (!formData.account) {
    errors.account = FlowFormError.accountRequired;
  }

  if (!formData.category) {
    errors.category = FlowFormError.categoryRequired;
  }

  return errors;
};

const saveFlow = async (repositories: RepositoriesState, formData: FlowFormData): Promise<Result<unknown>> => {
  const result = ModifyFlowCommand.create({
    id: formData.id,
    date: formData.date,
    amount: formData.amount,
    description: formData.description,
    accountId: formData.account.id,
    categoryId: formData.category.id,
    tags: formData.tags.map((tag) => tag.tag),
  });
  if (result.isFailure) return result;

  const useCase = ModifyFlowUseCase(repositories.flowRepository);
  return await useCase.execute(result.value);
};

export const { FormDataProvider: FlowFormProvider, useFormDataContext: useFlowForm } = createFormDataContext<FlowFormData, FlowFormError>(
  'Flow',
  saveFlow
);
