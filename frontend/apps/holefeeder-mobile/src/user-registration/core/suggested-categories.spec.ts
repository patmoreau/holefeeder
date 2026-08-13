import { en, fr } from '@holefeeder/shared/core';
import { SuggestedCategoryColors, SuggestedCategoryKeys } from '@/user-registration/core/suggested-categories';

describe('suggested categories', () => {
  it('offers a colour for every suggestion', () => {
    expect(Object.keys(SuggestedCategoryColors).sort()).toEqual([...SuggestedCategoryKeys].sort());
  });

  it('gives each suggestion its own colour', () => {
    const colours = Object.values(SuggestedCategoryColors);

    expect(new Set(colours).size).toBe(colours.length);
  });

  it('is translated in both locales', () => {
    for (const key of SuggestedCategoryKeys) {
      expect(en.onboarding.suggestedCategories[key]).toBeTruthy();
      expect(fr.onboarding.suggestedCategories[key]).toBeTruthy();
    }
  });

  it('does not suggest a name the transfer feature owns', () => {
    const suggestions = SuggestedCategoryKeys.map((key) => en.onboarding.suggestedCategories[key]);

    expect(suggestions).not.toContain('Transfer In');
    expect(suggestions).not.toContain('Transfer Out');
  });
});
