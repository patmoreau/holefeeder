import { Language } from '@/shared/i18n/language';

export const percent = (value: number) =>
  new Intl.NumberFormat(Language.locale(), { style: 'percent', maximumFractionDigits: 0 }).format(value / 100);
