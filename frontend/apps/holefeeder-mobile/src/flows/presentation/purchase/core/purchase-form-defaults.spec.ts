import { Result, today } from '@holefeeder/shared/core';
import { anAccount, toAccount } from '@/accounts/core/__tests__/account-for-test';
import { aCategory, toCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { PurchaseFormData, PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { PurchaseFormDefaults, PurchaseFormDefaultsErrors } from '@/flows/presentation/purchase/core/purchase-form-defaults';
import { CategoryTypes } from '@/shared/core/category-type';
import { System } from '@/shared/core/system';

const accounts = () => anAccount().times(3).map(toAccount);
const categories = () => aCategory({ type: CategoryTypes.expense }).times(2).map(toCategory);

const formDataOf = (result: Result<PurchaseFormData>): PurchaseFormData => {
  if (result.isFailure) throw new Error(`Expected form data, got ${result.errors}`);
  return result.value;
};

describe('PurchaseFormDefaults', () => {
  it('should select the requested account as the source', () => {
    const allAccounts = accounts();
    const requested = allAccounts[2];

    const formData = formDataOf(PurchaseFormDefaults.create({ accounts: allAccounts, categories: categories(), accountId: requested.id }));

    expect(formData.sourceAccount).toBe(requested);
  });

  it('should select the first account when no account is requested', () => {
    const allAccounts = accounts();

    const formData = formDataOf(PurchaseFormDefaults.create({ accounts: allAccounts, categories: categories() }));

    expect(formData.sourceAccount).toBe(allAccounts[0]);
  });

  it('should select the first account when the requested account is unknown', () => {
    const allAccounts = accounts();

    const formData = formDataOf(PurchaseFormDefaults.create({ accounts: allAccounts, categories: categories(), accountId: anAccount().id }));

    expect(formData.sourceAccount).toBe(allAccounts[0]);
  });

  it('should target an account other than the source', () => {
    const allAccounts = accounts();
    const requested = allAccounts[0];

    const formData = formDataOf(PurchaseFormDefaults.create({ accounts: allAccounts, categories: categories(), accountId: requested.id }));

    expect(formData.targetAccount).not.toBe(requested);
    expect(allAccounts).toContain(formData.targetAccount);
  });

  it('should target the source account when it is the only one', () => {
    const [only] = anAccount().times(1).map(toAccount);

    const formData = formDataOf(PurchaseFormDefaults.create({ accounts: [only], categories: categories(), accountId: only.id }));

    expect(formData.targetAccount).toBe(only);
  });

  it('should select the first expense category that is not a system category', () => {
    const system = toCategory(aCategory({ type: CategoryTypes.expense, system: true as System }));
    const gain = toCategory(aCategory({ type: CategoryTypes.gain }));
    const expense = toCategory(aCategory({ type: CategoryTypes.expense }));

    const formData = formDataOf(PurchaseFormDefaults.create({ accounts: accounts(), categories: [system, gain, expense] }));

    expect(formData.category).toBe(expense);
  });

  it('should keep the remaining defaults', () => {
    const allCategories = categories();

    const formData = formDataOf(PurchaseFormDefaults.create({ accounts: accounts(), categories: allCategories }));

    expect(formData).toMatchObject({
      purchaseType: PurchaseType.expense,
      date: today(),
      amount: 0,
      description: '',
      category: allCategories[0],
      tags: [],
      hasCashflow: false,
      cashflowEffectiveDate: today(),
      cashflowIntervalType: 'monthly',
      cashflowFrequency: 1,
    });
  });

  it('should ask for an account when there is none', () => {
    const result = PurchaseFormDefaults.create({ accounts: [], categories: categories() });

    expect(result).toBeFailureWithErrors([PurchaseFormDefaultsErrors.missingAccount]);
  });

  it('should ask for a category when there is none', () => {
    const result = PurchaseFormDefaults.create({ accounts: accounts(), categories: [] });

    expect(result).toBeFailureWithErrors([PurchaseFormDefaultsErrors.missingCategory]);
  });

  it('should ask for a category when only system and income categories exist', () => {
    const system = toCategory(aCategory({ type: CategoryTypes.expense, system: true as System }));
    const gain = toCategory(aCategory({ type: CategoryTypes.gain }));

    const result = PurchaseFormDefaults.create({ accounts: accounts(), categories: [system, gain] });

    expect(result).toBeFailureWithErrors([PurchaseFormDefaultsErrors.missingCategory]);
  });

  it('should ask for both when there is neither an account nor a category', () => {
    const result = PurchaseFormDefaults.create({ accounts: [], categories: [] });

    expect(result).toBeFailureWithErrors([PurchaseFormDefaultsErrors.missingAccount, PurchaseFormDefaultsErrors.missingCategory]);
  });
});
