import { DateOnly, DateOnlyErrors, Id, IdErrors, Money, MoneyErrors } from '@holefeeder/shared/core';
import { aPastDate, aRecentDate, anAmount, anId } from '@holefeeder/shared/testkit';
import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';

describe('PayFlowCommand', () => {
  let form: Record<string, unknown>;

  beforeEach(() => {
    form = {
      date: aRecentDate(),
      amount: anAmount(),
      cashflowId: anId(),
      cashflowDate: aPastDate(),
    };
  });

  it('succeeds with valid data', () => {
    const result = PayFlowCommand.create(form);
    expect(result).toBeSuccessWithValue({
      date: DateOnly.valid(form.date),
      amount: Money.valid(form.amount),
      cashflowId: Id.valid(form.cashflowId),
      cashflowDate: DateOnly.valid(form.cashflowDate),
      updateRecurringAmount: false,
    });
  });

  it('defaults updateRecurringAmount to false when absent', () => {
    const result = PayFlowCommand.create(form);
    expect(result.isSuccess && result.value.updateRecurringAmount).toBe(false);
  });

  it('captures updateRecurringAmount when provided', () => {
    form.updateRecurringAmount = true;
    const result = PayFlowCommand.create(form);
    expect(result.isSuccess && result.value.updateRecurringAmount).toBe(true);
  });

  it('returns failure if date is invalid', () => {
    form.date = 'invalid-date';
    const result = PayFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if amount is invalid', () => {
    form.amount = NaN;
    const result = PayFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('returns failure if cashflowId is invalid', () => {
    form.cashflowId = '';
    const result = PayFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if cashflowDate is invalid', () => {
    form.cashflowDate = 'invalid-date';
    const result = PayFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });
});
