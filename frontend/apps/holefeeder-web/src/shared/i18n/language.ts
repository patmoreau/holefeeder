import { LanguageType } from '@holefeeder/shared/core';

const locales: Record<LanguageType, string> = { [LanguageType.en]: 'en-CA', [LanguageType.fr]: 'fr-CA' };

const current = (): LanguageType => LanguageType.en;

const locale = (): string => locales[current()];

export const Language = {
  current: current,
  locale: locale,
};
