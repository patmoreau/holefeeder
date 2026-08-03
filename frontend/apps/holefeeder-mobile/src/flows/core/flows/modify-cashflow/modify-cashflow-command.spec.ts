import { DateIntervalType, DateIntervalTypes, DateOnly, DateOnlyErrors, Id, IdErrors, Money, MoneyErrors } from '@holefeeder/shared/core';
import { ModifyCashflowCommand, ModifyCashflowErrors } from '@/flows/core/flows/modify-cashflow/modify-cashflow-command';
import { TagList } from '@/flows/core/flows/tag-list';
import { aPastDate } from '@/shared/__tests__/date-for-test';
import { anAmount } from '@/shared/__tests__/number-for-test';
import { anId } from '@/shared/__tests__/string-for-test';

describe('ModifyCashflowCommand', () => {
  let form: Record<string, unknown>;

  beforeEach(() => {
    form = {
      id: anId(),
      effectiveDate: aPastDate(),
      amount: anAmount(),
      intervalType: DateIntervalTypes.monthly as DateIntervalType,
      frequency: 1,
      recurrence: 12,
      description: 'Car insurance',
      accountId: anId(),
      categoryId: anId(),
      tags: ['bills'],
    };
  });

  it('succeeds with valid data', () => {
    const result = ModifyCashflowCommand.create(form);
    expect(result).toBeSuccessWithValue({
      id: Id.valid(form.id),
      effectiveDate: DateOnly.valid(form.effectiveDate),
      amount: Money.valid(form.amount),
      intervalType: form.intervalType,
      frequency: 1,
      recurrence: 12,
      description: 'Car insurance',
      accountId: Id.valid(form.accountId),
      categoryId: Id.valid(form.categoryId),
      tags: TagList.valid(['bills']),
    });
  });

  it('trims tags and filters empty ones', () => {
    form.tags = [' bills ', '', 'auto '];
    const result = ModifyCashflowCommand.create(form);
    expect(result).toBeSuccessWithValue(expect.objectContaining({ tags: ['bills', 'auto'] }));
  });

  it('defaults recurrence to zero when absent', () => {
    delete form.recurrence;
    const result = ModifyCashflowCommand.create(form);
    expect(result.isSuccess && result.value.recurrence).toBe(0);
  });

  it('returns failure if effectiveDate is invalid', () => {
    form.effectiveDate = 'invalid-date';
    const result = ModifyCashflowCommand.create(form);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if amount is invalid', () => {
    form.amount = NaN;
    const result = ModifyCashflowCommand.create(form);
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('returns failure if frequency is below one', () => {
    form.frequency = 0;
    const result = ModifyCashflowCommand.create(form);
    expect(result).toBeFailureWithErrors([ModifyCashflowErrors.invalidFrequency]);
  });

  it('returns failure if accountId is invalid', () => {
    form.accountId = '';
    const result = ModifyCashflowCommand.create(form);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if categoryId is invalid', () => {
    form.categoryId = '';
    const result = ModifyCashflowCommand.create(form);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });
});
