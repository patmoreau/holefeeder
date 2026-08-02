import { DateOnly, Id, Money, Result } from '@holefeeder/shared/core';

export type PayFlowCommand = {
  date: DateOnly;
  amount: Money;
  cashflowId: Id;
  cashflowDate: DateOnly;
  updateRecurringAmount: boolean;
};

const create = (pay: Record<string, unknown>): Result<PayFlowCommand> =>
  Result.combine<PayFlowCommand>({
    date: DateOnly.create(pay.date),
    amount: Money.create(pay.amount),
    cashflowId: Id.create(pay.cashflowId),
    cashflowDate: DateOnly.create(pay.cashflowDate),
    updateRecurringAmount: Result.success((pay.updateRecurringAmount as boolean) ?? false),
  });

const valid = (pay: Record<string, unknown>): PayFlowCommand => ({
  date: DateOnly.valid(pay.date),
  amount: Money.valid(pay.amount),
  cashflowId: Id.valid(pay.cashflowId),
  cashflowDate: DateOnly.valid(pay.cashflowDate),
  updateRecurringAmount: (pay.updateRecurringAmount as boolean) ?? false,
});

export const PayFlowCommand = {
  create: create,
  valid: valid,
};
