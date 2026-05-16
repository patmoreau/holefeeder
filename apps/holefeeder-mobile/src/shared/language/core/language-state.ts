import { LanguageType } from '@/shared/language/core/language-type';

export type LanguageState = {
  language: LanguageType;
  setLanguage: (language: LanguageType) => void;
  availableLanguages: { code: LanguageType; name: string }[];
};
