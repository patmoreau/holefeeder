import { tk } from '../translations/translations';
import { LanguageResolver } from './language-resolver';
import { LanguageType } from './language-type';

describe('LanguageResolver', () => {
  describe('resolve', () => {
    it('uses the stored language', () => {
      expect(LanguageResolver.resolve('fr', ['en-US'])).toBe('fr');
    });

    it('prefers the stored language over the device', () => {
      expect(LanguageResolver.resolve('en', ['fr-CA'])).toBe('en');
    });

    it('follows the device when nothing is stored', () => {
      expect(LanguageResolver.resolve(null, ['fr-CA', 'en-CA'])).toBe('fr');
    });

    it('takes the first supported device language', () => {
      expect(LanguageResolver.resolve(null, ['de-DE', 'fr', 'en'])).toBe('fr');
    });

    it('ignores an unknown stored language', () => {
      expect(LanguageResolver.resolve('de', ['fr-CA'])).toBe('fr');
    });

    it('falls back to English', () => {
      expect(LanguageResolver.resolve(undefined, ['de-DE'])).toBe('en');
    });
  });

  describe('localeOf', () => {
    it('maps each language to its Canadian locale', () => {
      expect(LanguageResolver.localeOf(LanguageType.en)).toBe('en-CA');
      expect(LanguageResolver.localeOf(LanguageType.fr)).toBe('fr-CA');
    });
  });

  describe('labelKey', () => {
    it('names each language with its display key', () => {
      expect(LanguageResolver.labelKey).toEqual({ en: tk.displaySection.english, fr: tk.displaySection.french });
    });
  });
});
