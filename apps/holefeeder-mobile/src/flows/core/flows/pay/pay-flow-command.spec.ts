import { PayFlowCommand } from '@/flows/core/flows/pay/pay-flow-command';
import { aPastDate, aRecentDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId } from '@/shared/__tests__/string-for-test';
import { DateOnly, DateOnlyErrors } from '@/shared/core/date-only';
import { Id, IdErrors } from '@/shared/core/id';
import { Money, MoneyErrors } from '@/shared/core/money';

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
    });
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
