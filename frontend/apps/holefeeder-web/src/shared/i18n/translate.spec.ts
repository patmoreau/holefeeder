import { tk } from '@holefeeder/shared/core';
import { describe, expect, it } from 'vitest';
import { translate } from './translate';

describe('translate', () => {
  it('ShouldReturnTheEnglishString', () => {
    expect(translate(tk.dashboard.largeHeader.netFlow)).toBe('Net Flow');
  });

  it('ShouldReplacePlaceholdersWithValues', () => {
    expect(translate(tk.dashboard.largeHeader.variation, { variation: '$212.40' })).toBe('$212.40 vs Average');
  });

  it('ShouldReplacePlaceholdersWithNumbers', () => {
    expect(translate(tk.common.last7Days, { count: 3 })).toBe('3 days ago');
  });

  it('ShouldLeavePlaceholdersWithoutValues', () => {
    expect(translate(tk.dashboard.largeHeader.variation)).toBe('{{variation}} vs Average');
  });

  it('ShouldReturnTheKeyWhenMissing', () => {
    expect(translate('dashboard.unknown')).toBe('dashboard.unknown');
  });
});
