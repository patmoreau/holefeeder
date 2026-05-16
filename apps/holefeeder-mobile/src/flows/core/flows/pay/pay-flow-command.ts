import { DateOnly, Id, Money, Result } from '@holefeeder/core';

export type PayFlowCommand = {
  date: DateOnly;
  amount: Money;
  cashflowId: Id;
  cashflowDate: DateOnly;
};

const create = (pay: Record<string, unknown>): Result<PayFlowCommand> =>
  Result.combine<PayFlowCommand>({
    date: DateOnly.create(pay.date),
    amount: Money.create(pay.amount),
    cashflowId: Id.create(pay.cashflowId),
    cashflowDate: DateOnly.create(pay.cashflowDate),
  });

const valid = (pay: Record<string, unknown>): PayFlowCommand => ({
  date: DateOnly.valid(pay.date),
  amount: Money.valid(pay.amount),
  cashflowId: Id.valid(pay.cashflowId),
  cashflowDate: DateOnly.valid(pay.cashflowDate),
});

export const PayFlowCommand = {
  create: create,
  valid: valid,
};
