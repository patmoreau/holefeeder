import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';
import { PayUseCase } from '@/flows/core/flows/pay/pay-use-case';
import { PayUpcomingFormData } from '@/flows/presentation/pay-upcoming/core/pay-upcoming-form-data';
import { Money } from '@/shared/core/money';
import { Result } from '@/shared/core/result';
import { createFormDataContext, ValidationFunction } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const PayUpcomingFormError = {
  amountRequired: 'amountRequired',
} as const;

export type PayUpcomingFormError = (typeof PayUpcomingFormError)[keyof typeof PayUpcomingFormError];

export const validatePayUpcomingForm: ValidationFunction<PayUpcomingFormData, PayUpcomingFormError> = (formData) => {
  const errors: Partial<Record<keyof PayUpcomingFormData, PayUpcomingFormError>> = {};

  if (Money.create(formData.amount).isFailure) {
    errors.amount = PayUpcomingFormError.amountRequired;
  }
  return errors;
};

const payUpcoming = async (repositories: RepositoriesState, formData: PayUpcomingFormData): Promise<Result<unknown>> => {
  const result = PayFlowCommand.create({
    cashflowId: formData.cashflowId,
    cashflowDate: formData.cashflowDate,
    date: formData.date,
    amount: formData.amount,
  });

  if (result.isFailure) return result;

  const useCase = PayUseCase(repositories.flowRepository);
  return await useCase.execute(result.value);
};

export const { FormDataProvider: PayUpcomingFormProvider, useFormDataContext: usePayUpcomingForm } = createFormDataContext<
  PayUpcomingFormData,
  PayUpcomingFormError
>('PayUpcoming', payUpcoming);
