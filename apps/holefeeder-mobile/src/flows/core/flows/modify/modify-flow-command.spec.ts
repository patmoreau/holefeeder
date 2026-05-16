import { aFlowForm } from '@/flows/core/flows/__tests__/flow-form-for-test';
import { ModifyFlowCommand } from '@/flows/core/flows/modify/modify-flow-command';
import { DateOnly, DateOnlyErrors } from '@/shared/core/date-only';
import { Id, IdErrors } from '@/shared/core/id';
import { Money, MoneyErrors } from '@/shared/core/money';

describe('ModifyFlowCommand', () => {
  it('succeeds with valid data', () => {
    const form = aFlowForm();
    const result = ModifyFlowCommand.create(form);
    expect(result).toBeSuccessWithValue({
      id: Id.valid(form.id),
      date: DateOnly.valid(form.date),
      amount: Money.valid(form.amount),
      description: form.description,
      accountId: Id.valid(form.accountId),
      categoryId: Id.valid(form.categoryId),
      tags: form.tags,
    });
  });

  it('trims tags and filters empty ones', async () => {
    const form = aFlowForm({ tags: [' tag1 ', '', 'tag2 '] });
    const result = ModifyFlowCommand.create(form);
    expect(result).toBeSuccessWithValue(
      expect.objectContaining({
        tags: ['tag1', 'tag2'],
      })
    );
  });

  it('returns failure if date is invalid', () => {
    const form = aFlowForm({ date: 'invalid-date' });
    const result = ModifyFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if amount is invalid', () => {
    const form = aFlowForm({ amount: NaN });
    const result = ModifyFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('returns failure if accountId is invalid', () => {
    const form = aFlowForm({ accountId: '' });
    const result = ModifyFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if categoryId is invalid', () => {
    const form = aFlowForm({ categoryId: '' });
    const result = ModifyFlowCommand.create(form);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });
});
