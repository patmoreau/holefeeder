import { DateIntervalTypes, DateOnly } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { PeriodTotals } from './period-series';

const yearOf = (date: DateOnly) => date.slice(0, 4);

const format = (date: DateOnly, locale: string, options: Intl.DateTimeFormatOptions) =>
  new Intl.DateTimeFormat(locale, { timeZone: 'UTC', ...options }).format(new Date(`${date}T00:00:00Z`));

const forSeries = (series: PeriodTotals[], settings: Settings, locale: string): string[] => {
  const byMonth = settings.intervalType === DateIntervalTypes.monthly && settings.frequency === 1;
  const crossesYear = new Set(series.map((period) => yearOf(period.from))).size > 1;

  return series.map((period, index) => {
    const showYear = crossesYear && (index === 0 || yearOf(period.from) !== yearOf(series[index - 1].from));
    const options: Intl.DateTimeFormatOptions = byMonth ? { month: 'short' } : { month: 'short', day: 'numeric' };

    return format(period.from, locale, showYear ? { ...options, year: 'numeric' } : options);
  });
};

export const PeriodLabel = {
  forSeries: forSeries,
};
