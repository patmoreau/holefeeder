import { TransferFlowCommand } from '@/flows/core/flows/transfer/transfer-flow-command';
import { aString } from '@/shared/__tests__/string-for-test';
import { DateOnly, DateOnlyErrors } from '@/shared/core/date-only';
import { Id, IdErrors } from '@/shared/core/id';
import { Money, MoneyErrors } from '@/shared/core/money';

describe('TransferFlowCommand', () => {
  const transfer = {
    date: '2024-01-01',
    amount: 123.45,
    description: aString(),
    sourceAccountId: '3855439b-bc3c-42e4-aabe-c41ff3944f2a',
    targetAccountId: '47e9ad0f-699b-4687-8369-3ac39843df7b',
  };

  it('succeeds with valid data', () => {
    const result = TransferFlowCommand.create(transfer);
    expect(result).toBeSuccessWithValue({
      date: DateOnly.valid(transfer.date),
      amount: Money.valid(transfer.amount),
      description: transfer.description,
      sourceAccountId: Id.valid(transfer.sourceAccountId),
      targetAccountId: Id.valid(transfer.targetAccountId),
    });
  });

  it('returns failure if date is invalid', () => {
    const badTransfer = {
      ...transfer,
      date: 'invalid-date',
    };
    const result = TransferFlowCommand.create(badTransfer);
    expect(result).toBeFailureWithErrors([DateOnlyErrors.invalid]);
  });

  it('returns failure if amount is invalid', () => {
    const badTransfer = {
      ...transfer,
      amount: NaN,
    };
    const result = TransferFlowCommand.create(badTransfer);
    expect(result).toBeFailureWithErrors([MoneyErrors.invalid]);
  });

  it('returns failure if sourceAccountId is invalid', () => {
    const badTransfer = {
      ...transfer,
      sourceAccountId: '',
    };
    const result = TransferFlowCommand.create(badTransfer);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });

  it('returns failure if targetAccountId is invalid', () => {
    const badTransfer = {
      ...transfer,
      targetAccountId: '',
    };
    const result = TransferFlowCommand.create(badTransfer);
    expect(result).toBeFailureWithErrors([IdErrors.invalid]);
  });
});
