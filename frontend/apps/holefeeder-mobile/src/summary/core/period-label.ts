import { DateIntervalTypes, DateOnly, LocalFormatter } from '@holefeeder/shared/core';
import { Settings } from '@/shared/core/settings';
import { PeriodTotals } from './period-series';

const yearOf = (date: DateOnly) => date.slice(0, 4);

const format = (date: DateOnly, locale: string, options: Intl.DateTimeFormatOptions) =>
  new Intl.DateTimeFormat(locale, { timeZone: 'UTC', ...options }).format(new Date(`${date}T00:00:00Z`));

const isOneMonth = (settings: Settings) => settings.intervalType === DateIntervalTypes.monthly && settings.frequency === 1;

const forSeries = (series: PeriodTotals[], settings: Settings, locale: string, maxLabels: number = series.length): string[] => {
  const byMonth = isOneMonth(settings);
  const step = Math.max(1, Math.ceil(series.length / Math.max(1, maxLabels)));
  const lastIndex = series.length - 1;
  const visible = series.map((_, index) => (lastIndex - index) % step === 0);
  const visibleYears = series.filter((_, index) => visible[index]).map((period) => yearOf(period.from));
  const crossesYear = new Set(visibleYears).size > 1;

  let previousYear: string | undefined;
  return series.map((period, index) => {
    if (!visible[index]) return '';

    const year = yearOf(period.from);
    const showYear = crossesYear && year !== previousYear;
    previousYear = year;
    const options: Intl.DateTimeFormatOptions = byMonth ? { month: 'short' } : { month: 'short', day: 'numeric' };

    return format(period.from, locale, showYear ? { ...options, year: 'numeric' } : options);
  });
};

const forTooltip = (period: PeriodTotals, settings: Settings, locale: string): string =>
  isOneMonth(settings)
    ? format(period.from, locale, { month: 'long', year: 'numeric' })
    : LocalFormatter.dateRange(period.from, period.to, locale);

export const PeriodLabel = {
  forSeries: forSeries,
  forTooltip: forTooltip,
};
