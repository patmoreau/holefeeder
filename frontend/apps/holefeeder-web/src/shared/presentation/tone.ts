import { AmountTone, type DateOnly, LocalFormatter } from '@holefeeder/shared/core';
import { Language } from '@/shared/i18n/language';

export const currency = (amount: number) => LocalFormatter.currency(amount, Language.locale(), 'CAD');

export const mediumDate = (date: DateOnly) =>
  new Intl.DateTimeFormat(Language.locale(), { dateStyle: 'medium', timeZone: 'UTC' }).format(new Date(date));

export const unsignedCurrency = (value: number) => currency(AmountTone.display(value).amount);

export const toneColor: Record<AmountTone, string> = { positive: 'success.main', negative: 'error.main', neutral: 'text.primary' };
