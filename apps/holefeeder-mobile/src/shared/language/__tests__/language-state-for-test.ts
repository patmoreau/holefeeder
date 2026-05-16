import { LanguageState } from '@/shared/language/core/language-state';
import { LanguageType } from '@/shared/language/core/language-type';

const defaultState = (): LanguageState => ({
  language: LanguageType.en,
  setLanguage: (language: LanguageType) => {},
  availableLanguages: [
    { code: LanguageType.en, name: 'English' },
    { code: LanguageType.fr, name: 'Français' },
  ],
});

export const aLanguageState = (overrides?: Partial<LanguageState>) => ({
  ...defaultState(),
  ...overrides,
});
