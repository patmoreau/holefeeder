import { describe, expect, it } from 'vitest';
import { Language } from './language';

describe('Language', () => {
  it('ShouldUseTheStoredLanguage', () => {
    expect(Language.resolve('fr', ['en-US'])).toBe('fr');
  });

  it('ShouldPreferTheStoredLanguageOverTheBrowser', () => {
    expect(Language.resolve('en', ['fr-CA'])).toBe('en');
  });

  it('ShouldFollowTheBrowserWhenNothingIsStored', () => {
    expect(Language.resolve(null, ['fr-CA', 'en-CA'])).toBe('fr');
  });

  it('ShouldTakeTheFirstSupportedBrowserLanguage', () => {
    expect(Language.resolve(null, ['de-DE', 'fr', 'en'])).toBe('fr');
  });

  it('ShouldIgnoreAnUnknownStoredLanguage', () => {
    expect(Language.resolve('de', ['fr-CA'])).toBe('fr');
  });

  it('ShouldFallBackToEnglish', () => {
    expect(Language.resolve(null, ['de-DE'])).toBe('en');
  });

  it('ShouldMapEachLanguageToItsCanadianLocale', () => {
    expect(Language.localeOf('en')).toBe('en-CA');
    expect(Language.localeOf('fr')).toBe('fr-CA');
  });
});
