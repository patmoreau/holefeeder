import { LanguageType } from '@holefeeder/shared/core';

const storageKey = 'holefeeder.language';

const locales: Record<LanguageType, string> = { [LanguageType.en]: 'en-CA', [LanguageType.fr]: 'fr-CA' };

const supported = (value: string | null | undefined): LanguageType | undefined => {
  const language = value?.toLowerCase().split('-')[0];
  return Object.values(LanguageType).find((type) => type === language);
};

const resolve = (stored: string | null, browserLanguages: readonly string[]): LanguageType =>
  supported(stored) ?? browserLanguages.map(supported).find((language) => language !== undefined) ?? LanguageType.en;

const stored = (): string | null => {
  try {
    return localStorage.getItem(storageKey);
  } catch {
    return null;
  }
};

const current = (): LanguageType => (typeof window === 'undefined' ? LanguageType.en : resolve(stored(), navigator.languages));

const choose = (language: LanguageType) => {
  try {
    localStorage.setItem(storageKey, language);
  } catch {
    return;
  }
};

const localeOf = (language: LanguageType): string => locales[language];

const locale = (): string => localeOf(current());

export const Language = {
  resolve: resolve,
  current: current,
  choose: choose,
  localeOf: localeOf,
  locale: locale,
};
