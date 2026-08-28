import { today } from '@holefeeder/shared/core';
import { anAccount, toAccount } from '@/accounts/core/__tests__/account-for-test';
import { aCategory, toCategory } from '@/flows/core/categories/__tests__/category-for-test';
import { PurchaseType } from '@/flows/presentation/purchase/core/purchase-form-data';
import { PurchaseFormDefaults } from '@/flows/presentation/purchase/core/purchase-form-defaults';

const accounts = () => anAccount().times(3).map(toAccount);
const categories = () => aCategory().times(2).map(toCategory);

describe('PurchaseFormDefaults', () => {
  it('should select the first account as the source', () => {
    const allAccounts = accounts();

    const formData = PurchaseFormDefaults.create({ accounts: allAccounts, categories: categories() });

    expect(formData.sourceAccount).toBe(allAccounts[0]);
  });

  it('should target an account other than the source', () => {
    const allAccounts = accounts();

    const formData = PurchaseFormDefaults.create({ accounts: allAccounts, categories: categories() });

    expect(formData.targetAccount).toBe(allAccounts[1]);
  });

  it('should target the source account when it is the only one', () => {
    const [only] = anAccount().times(1).map(toAccount);

    const formData = PurchaseFormDefaults.create({ accounts: [only], categories: categories() });

    expect(formData.targetAccount).toBe(only);
  });

  it('should keep the remaining defaults', () => {
    const allCategories = categories();

    const formData = PurchaseFormDefaults.create({ accounts: accounts(), categories: allCategories });

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
});
