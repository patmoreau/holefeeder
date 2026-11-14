import { DateIntervalType, tk } from '@holefeeder/shared/core';
import type { Settings } from '@holefeeder/shared/core';
import { translate } from '@/shared/i18n/translate';
import { mediumDate } from '@/shared/presentation/tone';

export const budgetSummary = (settings: Settings) =>
  translate(tk.budgetSettings.summary, {
    period: DateIntervalType.cadence(settings.intervalType, settings.frequency, translate),
    date: mediumDate(settings.effectiveDate),
  });
