import { type DateOnly, LocalFormatter } from '@holefeeder/shared/core';
import { Language } from '@/shared/i18n/language';

export type Tone = 'positive' | 'negative' | 'neutral';

export const currency = (amount: number) => LocalFormatter.currency(amount, Language.locale(), 'CAD');

export const mediumDate = (date: DateOnly) =>
  new Intl.DateTimeFormat(Language.locale(), { dateStyle: 'medium', timeZone: 'UTC' }).format(new Date(date));

export const roundsToZero = (value: number) => Math.round(Math.abs(value) * 100) === 0;

export const unsignedCurrency = (value: number) => currency(roundsToZero(value) ? 0 : Math.abs(value));

export const toneOf = (value: number): Tone => {
  if (roundsToZero(value)) return 'neutral';
  return value > 0 ? 'positive' : 'negative';
};

export const toneColor: Record<Tone, string> = { positive: 'success.main', negative: 'error.main', neutral: 'text.primary' };
