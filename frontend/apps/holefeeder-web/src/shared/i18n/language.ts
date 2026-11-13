import { LanguageResolver, LanguageType } from '@holefeeder/shared/core';

const storageKey = 'holefeeder.language';

const stored = (): string | null => {
  try {
    return localStorage.getItem(storageKey);
  } catch {
    return null;
  }
};

const current = (): LanguageType => (typeof window === 'undefined' ? LanguageType.en : LanguageResolver.resolve(stored(), navigator.languages));

const choose = (language: LanguageType) => {
  try {
    localStorage.setItem(storageKey, language);
  } catch {
    return;
  }
};

const locale = (): string => LanguageResolver.localeOf(current());

export const Language = {
  current: current,
  choose: choose,
  locale: locale,
};
