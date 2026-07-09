import { differenceInCalendarDays, parseISO } from 'date-fns';
import { DateOnly } from './date-only';
import { tk } from './translations/translations';
import { withDate } from './with-date';

type TranslateFn = (key: string, options?: Record<string, unknown>) => string;

const percentage = (val: number): string => `${val.toFixed(2)}%`;

const currency = (
  amount: number,
  locale: string,
  currencyCode: string,
  options?: {
    currencyDisplay?: Intl.NumberFormatOptions['currencyDisplay'];
    style?: Intl.NumberFormatOptions['style'];
  }
): string => {
  const safeCurrencyCode = currencyCode || 'CAD';
  try {
    return new Intl.NumberFormat(locale, {
      currency: safeCurrencyCode,
      style: options?.style ?? 'currency',
      currencyDisplay: options?.currencyDisplay ?? 'narrowSymbol',
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    }).format(amount);
  } catch {
    return `${amount} ${safeCurrencyCode}`;
  }
};

const date = (dateValue: DateOnly, anchorDate: DateOnly, locale: string, t: TranslateFn, options?: Intl.DateTimeFormatOptions): string => {
  try {
    const dateObj = parseISO(dateValue);
    const anchor = parseISO(anchorDate);
    const diffDays = differenceInCalendarDays(anchor, dateObj);

    if (diffDays === 0) return t(tk.common.today as string);
    if (diffDays === 1) return t(tk.common.yesterday as string);
    if (diffDays === -1) return t(tk.common.tomorrow as string);
    if (diffDays > 1 && diffDays < 8) return t(tk.common.last7Days as string, { count: diffDays });
    if (diffDays < -1 && diffDays > -8) return t(tk.common.next7Days as string, { count: Math.abs(diffDays) });

    return new Intl.DateTimeFormat(locale, {
      dateStyle: 'medium',
      timeZone: 'UTC',
      ...options,
    }).format(dateObj);
  } catch {
    return withDate(dateValue).toDate().toDateString();
  }
};

export const LocalFormatter = {
  percentage,
  currency,
  date,
} as const;
