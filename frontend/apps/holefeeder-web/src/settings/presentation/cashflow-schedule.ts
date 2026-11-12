import { tk } from '@holefeeder/shared/core';
import { translate } from '@/shared/i18n/translate';
import { mediumDate } from '@/shared/presentation/tone';
import type { Cashflow } from '../domain/cashflow';
import { periodLabel } from './budget-summary';

export const scheduleOf = (cashflow: Cashflow) => {
  const period = periodLabel(cashflow.intervalType, cashflow.frequency);
  return cashflow.nextDate === null ? period : translate(tk.cashflows.next, { period: period, date: mediumDate(cashflow.nextDate) });
};
