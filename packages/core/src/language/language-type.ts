export const LanguageType = {
  en: 'en',
  fr: 'fr',
} as const;

export type LanguageType = (typeof LanguageType)[keyof typeof LanguageType];
