import { DateIntervalType, tk } from '@holefeeder/shared/core';
import { translate } from '@/shared/i18n/translate';
import { mediumDate } from '@/shared/presentation/tone';
import type { BudgetSettings } from '../domain/budget-settings';

export const budgetSummary = (settings: BudgetSettings) =>
  translate(tk.budgetSettings.summary, {
    period: DateIntervalType.cadence(settings.intervalType, settings.frequency, translate),
    date: mediumDate(settings.effectiveDate),
  });
