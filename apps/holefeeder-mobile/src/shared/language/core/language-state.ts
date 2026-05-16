import { LanguageType } from '@holefeeder/core';

export type LanguageState = {
  language: LanguageType;
  setLanguage: (language: LanguageType) => void;
  availableLanguages: { code: LanguageType; name: string }[];
};
