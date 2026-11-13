import { DateIntervalType, tk } from '@holefeeder/shared/core';
import { translate } from '@/shared/i18n/translate';
import { mediumDate } from '@/shared/presentation/tone';
import type { Cashflow } from '../domain/cashflow';

export const scheduleOf = (cashflow: Cashflow) => {
  const period = DateIntervalType.cadence(cashflow.intervalType, cashflow.frequency, translate);
  return cashflow.nextDate === null ? period : translate(tk.cashflows.next, { period: period, date: mediumDate(cashflow.nextDate) });
};
