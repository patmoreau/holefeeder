import { chipFontModifier } from './AppChip';

describe('chipFontModifier', () => {
  it('should use a text style so the chip scales with Dynamic Type', () => {
    expect(chipFontModifier()).toMatchObject({ textStyle: 'footnote' });
  });

  it('should omit the size so iOS resolves it from the text style', () => {
    expect(chipFontModifier()).not.toHaveProperty('size');
  });
});
