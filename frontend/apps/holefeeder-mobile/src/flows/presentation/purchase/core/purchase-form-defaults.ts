import { Id, today } from '@holefeeder/shared/core';
import { Account } from '@/accounts/core/account';
import { Category } from '@/flows/core/categories/category';
import { PurchaseFormData, PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';

export type PurchaseFormDefaultsParams = {
  accounts: Account[];
  categories: Category[];
  /** The account the purchase was started from, when there is one. */
  accountId?: Id;
};

const create = ({ accounts, categories, accountId }: PurchaseFormDefaultsParams): PurchaseFormData => {
  const sourceAccount = accounts.find((account) => account.id === accountId) ?? accounts[0];
  // A transfer to itself fails validation, so pick anything else when there is anything else.
  const targetAccount = accounts.find((account) => account.id !== sourceAccount?.id) ?? sourceAccount;

  return {
    purchaseType: PurchaseType.expense,
    date: today(),
    amount: 0,
    description: '',
    sourceAccount: sourceAccount,
    targetAccount: targetAccount,
    category: categories[0],
    tags: [],
    hasCashflow: false,
    cashflowEffectiveDate: today(),
    cashflowIntervalType: 'monthly',
    cashflowFrequency: 1,
  };
};

export const PurchaseFormDefaults = {
  create: create,
};
