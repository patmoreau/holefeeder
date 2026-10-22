import { DateIntervalTypes, DateOnly, LocalFormatter } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { PeriodTotals } from './period-series';

const format = (date: DateOnly, locale: string, options: Intl.DateTimeFormatOptions) =>
  new Intl.DateTimeFormat(locale, { timeZone: 'UTC', ...options }).format(new Date(`${date}T00:00:00Z`));

const isOneMonth = (settings: Settings) => settings.intervalType === DateIntervalTypes.monthly && settings.frequency === 1;

const forSeries = (series: PeriodTotals[], settings: Settings, locale: string, maxLabels: number = series.length): string[] => {
  const options: Intl.DateTimeFormatOptions = isOneMonth(settings) ? { month: 'short' } : { month: 'short', day: 'numeric' };
  const step = Math.max(1, Math.ceil(series.length / Math.max(1, maxLabels)));
  const lastIndex = series.length - 1;

  return series.map((period, index) => ((lastIndex - index) % step === 0 ? format(period.from, locale, options) : ''));
};

const forTooltip = (period: PeriodTotals, settings: Settings, locale: string): string =>
  isOneMonth(settings)
    ? format(period.from, locale, { month: 'long', year: 'numeric' })
    : LocalFormatter.dateRange(period.from, period.to, locale);

export const PeriodLabel = {
  forSeries: forSeries,
  forTooltip: forTooltip,
};
