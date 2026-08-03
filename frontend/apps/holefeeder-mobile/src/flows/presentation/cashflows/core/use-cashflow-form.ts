import { Money, Result } from '@holefeeder/shared/core';
import { ModifyCashflowCommand } from '@/flows/core/flows/modify-cashflow/modify-cashflow-command';
import { ModifyCashflowUseCase } from '@/flows/core/flows/modify-cashflow/modify-cashflow-use-case';
import { CashflowFormData } from '@/flows/presentation/cashflows/core/cashflow-form-data';
import { createFormDataContext, ValidationFunction } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const CashflowFormError = {
  amountRequired: 'amountRequired',
  accountRequired: 'accountRequired',
  categoryRequired: 'categoryRequired',
} as const;

export type CashflowFormError = (typeof CashflowFormError)[keyof typeof CashflowFormError];

export const validateCashflowForm: ValidationFunction<CashflowFormData, CashflowFormError> = (formData) => {
  const errors: Partial<Record<keyof CashflowFormData, CashflowFormError>> = {};

  if (Money.create(formData.amount).isFailure) {
    errors.amount = CashflowFormError.amountRequired;
  }

  if (!formData.account) {
    errors.account = CashflowFormError.accountRequired;
  }

  if (!formData.category) {
    errors.category = CashflowFormError.categoryRequired;
  }

  return errors;
};

const saveCashflow = async (repositories: RepositoriesState, formData: CashflowFormData): Promise<Result<unknown>> => {
  const result = ModifyCashflowCommand.create({
    id: formData.id,
    effectiveDate: formData.effectiveDate,
    amount: formData.amount,
    intervalType: formData.intervalType,
    frequency: formData.frequency,
    recurrence: formData.recurrence,
    description: formData.description,
    accountId: formData.account.id,
    categoryId: formData.category.id,
    tags: formData.tags.map((tag) => tag.tag),
  });
  if (result.isFailure) return result;

  const useCase = ModifyCashflowUseCase(repositories.flowRepository);
  return await useCase.execute(result.value);
};

export const { FormDataProvider: CashflowFormProvider, useFormDataContext: useCashflowForm } = createFormDataContext<
  CashflowFormData,
  CashflowFormError
>('Cashflow', saveCashflow);
