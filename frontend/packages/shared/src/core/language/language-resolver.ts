import { tk } from '../translations/translations';
import { LanguageType } from './language-type';

const locales: Record<LanguageType, string> = { [LanguageType.en]: 'en-CA', [LanguageType.fr]: 'fr-CA' };

const labelKey: Record<LanguageType, string> = {
  [LanguageType.en]: tk.displaySection.english,
  [LanguageType.fr]: tk.displaySection.french,
};

const supported = (value: string | null | undefined): LanguageType | undefined => {
  const language = value?.toLowerCase().split('-')[0];
  return Object.values(LanguageType).find((type) => type === language);
};

const resolve = (stored: string | null | undefined, deviceLanguages: readonly string[]): LanguageType =>
  supported(stored) ?? deviceLanguages.map(supported).find((language) => language !== undefined) ?? LanguageType.en;

const localeOf = (language: LanguageType): string => locales[language];

export const LanguageResolver = {
  resolve: resolve,
  localeOf: localeOf,
  labelKey: labelKey,
};
