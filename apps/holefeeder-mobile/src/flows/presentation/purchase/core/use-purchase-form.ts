import { CreateFlowCommand } from '@/flows/core/flows/create/create-flow-command';
import { CreateFlowUseCase } from '@/flows/core/flows/create/create-flow-use-case';
import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';
import { TransferFlowUseCase } from '@/flows/core/flows/transfer/transfer-flow-use-case';
import { PurchaseFormData, PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { Money } from '@/shared/core/money';
import { Result } from '@/shared/core/result';
import { createFormDataContext, ValidationFunction } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';

export const PurchaseFormError = {
  sameAccount: 'sameAccount',
  amountRequired: 'amountRequired',
  accountRequired: 'accountRequired',
  categoryRequired: 'categoryRequired',
} as const;

export type PurchaseFormError = (typeof PurchaseFormError)[keyof typeof PurchaseFormError];

export const validatePurchaseForm: ValidationFunction<PurchaseFormData, PurchaseFormError> = (formData) => {
  const errors: Partial<Record<keyof PurchaseFormData, PurchaseFormError>> = {};

  const transfer = formData.purchaseType === 'transfer';

  if (transfer && formData.sourceAccount && formData.targetAccount) {
    if (formData.sourceAccount.id === formData.targetAccount.id) {
      errors.targetAccount = PurchaseFormError.sameAccount;
    }
  }

  if (Money.create(formData.amount).isFailure) {
    errors.amount = PurchaseFormError.amountRequired;
  }

  if (!formData.sourceAccount) {
    errors.sourceAccount = PurchaseFormError.accountRequired;
  }

  if (!transfer && !formData.category) {
    errors.category = PurchaseFormError.categoryRequired;
  }

  return errors;
};

const savePurchase = async (repositories: RepositoriesState, formData: PurchaseFormData): Promise<Result<unknown>> => {
  const purchase = async (formData: PurchaseFormData): Promise<Result<unknown>> => {
    const cashflow = formData.hasCashflow
      ? {
          effectiveDate: formData.cashflowEffectiveDate,
          intervalType: formData.cashflowIntervalType,
          frequency: formData.cashflowFrequency,
          recurrence: 0,
        }
      : undefined;
    const result = CreateFlowCommand.create({
      date: formData.date,
      amount: formData.amount,
      description: formData.description,
      accountId: formData.sourceAccount.id,
      categoryId: formData.category.id,
      tags: formData.tags.map((tag) => tag.tag),
      cashflow,
    });
    if (result.isFailure) return result;

    const useCase = CreateFlowUseCase(repositories.flowRepository);
    return await useCase.execute(result.value);
  };

  const transfer = async (formData: PurchaseFormData): Promise<Result<unknown>> => {
    const result = TransferFlowCommand.create({
      date: formData.date,
      amount: formData.amount,
      description: formData.description,
      sourceAccountId: formData.sourceAccount.id,
      targetAccountId: formData.targetAccount.id,
    });
    if (result.isFailure) return result;

    const useCase = TransferFlowUseCase(repositories.flowRepository);
    return await useCase.execute(result.value);
  };

  if (formData.purchaseType === PurchaseType.transfer) return transfer(formData);
  return purchase(formData);
};

export const { FormDataProvider: PurchaseFormProvider, useFormDataContext: usePurchaseForm } = createFormDataContext<
  PurchaseFormData,
  PurchaseFormError
>('Purchase', savePurchase);
