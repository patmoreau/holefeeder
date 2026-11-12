import { tk } from '@holefeeder/shared/core';
import { translate } from '@/shared/i18n/translate';
import { mediumDate } from '@/shared/presentation/tone';
import type { BudgetIntervalType, BudgetSettings } from '../domain/budget-settings';

export const intervalLabelKey: Record<BudgetIntervalType, string> = {
  daily: tk.dateIntervalTypePicker.daily,
  weekly: tk.dateIntervalTypePicker.weekly,
  monthly: tk.dateIntervalTypePicker.monthly,
  yearly: tk.dateIntervalTypePicker.yearly,
  oneTime: tk.dateIntervalTypePicker.oneTime,
};

const unitKey: Partial<Record<BudgetIntervalType, string>> = {
  daily: tk.newTransaction.units.daily,
  weekly: tk.newTransaction.units.weekly,
  monthly: tk.newTransaction.units.monthly,
  yearly: tk.newTransaction.units.yearly,
};

export const periodLabel = (intervalType: BudgetIntervalType, frequency: number) => {
  const unit = unitKey[intervalType];
  return frequency === 1 || !unit
    ? translate(intervalLabelKey[intervalType])
    : translate(tk.budgetSettings.every, { count: frequency, unit: translate(unit) });
};

export const budgetSummary = (settings: BudgetSettings) =>
  translate(tk.budgetSettings.summary, {
    period: periodLabel(settings.intervalType, settings.frequency),
    date: mediumDate(settings.effectiveDate),
  });
