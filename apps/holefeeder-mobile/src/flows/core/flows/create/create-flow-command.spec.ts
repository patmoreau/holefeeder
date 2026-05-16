import { aPurchaseForm } from '@/flows/core/flows/__tests__/purchase-form-for-test';
import { CreateFlowCommand, CreateFlowErrors } from '@/flows/core/flows/create/create-flow-command';
import { DateOnly, DateOnlyErrors } from '@/shared/core/date-only';
import { Id, IdErrors } from '@/shared/core/id';
import { Money, MoneyErrors } from '@/shared/core/money';

describe('CreateFlowCommand', () => {
  it('succeeds with valid data', () => {
    const purchase = aPurchaseForm();
    const result = CreateFlowCommand.create(purchase);
    expect(result).toBeSuccessWithValue({
      date: DateOnly.valid(purchase.date),
      amount: Money.valid(purchase.amount),
      description: purchase.description,
      accountId: Id.valid(purchase.accountId),
      categoryId: Id.valid(purchase.categoryId),
      tags: purchase.tags,
      cashflow: purchase.cashflow,
    });
  });

  it('trims tags and filters empty ones', async () => {
    const purchase = aPurchaseForm({ tags: [' tag1 ', '', 'tag2 '] });
    const result = CreateFlowCommand.create(purchase);
    expect(result).toBeSuccessWithValue(
      expect.objectContaining({
        tags: ['tag1', 'tag2'],
      })
    );
  });

  it('returns failure if date is invalid', () => {
    const purchase = aPurchaseForm({ date: 'invalid-date' });
    const result = CreateFlowCommand.create(purchase);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if amount is invalid', () => {
    const makePurchase = aPurchaseForm({ amount: NaN });
    const result = CreateFlowCommand.create(makePurchase);
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('returns failure if accountId is invalid', () => {
    const makePurchase = aPurchaseForm({ accountId: '' });
    const result = CreateFlowCommand.create(makePurchase);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if categoryId is invalid', () => {
    const makePurchase = aPurchaseForm({ categoryId: '' });
    const result = CreateFlowCommand.create(makePurchase);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if cashflow effective date is invalid', () => {
    const makePurchase = aPurchaseForm({ cashflow: { effectiveDate: 'invalid-date' } });
    const result = CreateFlowCommand.create(makePurchase);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if cashflow frequency is invalid', () => {
    const makePurchase = aPurchaseForm({ cashflow: { frequency: -1 } });
    const result = CreateFlowCommand.create(makePurchase);
    expect(result).toBeFailureWithErrors([CreateFlowErrors.invalidCashflowFrequency]);
  });
});
