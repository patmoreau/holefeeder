import { dynamicTypeFontModifier, type ThemedTextVariant } from './AppText';

describe('dynamicTypeFontModifier', () => {
  it('should map the body variants to the body text style so they scale with Dynamic Type', () => {
    expect(dynamicTypeFontModifier('default')).toMatchObject({ textStyle: 'body' });
    expect(dynamicTypeFontModifier('defaultSemiBold')).toMatchObject({ textStyle: 'body' });
  });

  it('should map footnote to the iOS footnote text style rather than a caption size', () => {
    expect(dynamicTypeFontModifier('footnote')).toMatchObject({ textStyle: 'footnote' });
    expect(dynamicTypeFontModifier('errorField')).toMatchObject({ textStyle: 'footnote' });
  });

  it('should map the heading variants to their matching iOS text styles', () => {
    expect(dynamicTypeFontModifier('subtitle')).toMatchObject({ textStyle: 'subheadline' });
    expect(dynamicTypeFontModifier('title')).toMatchObject({ textStyle: 'title3' });
    expect(dynamicTypeFontModifier('largeTitle')).toMatchObject({ textStyle: 'largeTitle' });
  });

  it('should carry the variant font weight so the text style keeps its emphasis', () => {
    expect(dynamicTypeFontModifier('defaultSemiBold')).toMatchObject({ weight: 'semibold' });
    expect(dynamicTypeFontModifier('largeTitle')).toMatchObject({ weight: 'bold' });
  });

  it('should omit the size so iOS resolves it from the text style', () => {
    expect(dynamicTypeFontModifier('default')).not.toHaveProperty('size');
  });

  it('should return undefined for display, which is larger than any iOS text style', () => {
    expect(dynamicTypeFontModifier('display')).toBeUndefined();
  });

  it('should cover every variant', () => {
    const variants: ThemedTextVariant[] = [
      'default',
      'defaultSemiBold',
      'display',
      'errorField',
      'footnote',
      'largeTitle',
      'link',
      'subtitle',
      'title',
    ];

    variants.forEach((variant) => expect(() => dynamicTypeFontModifier(variant)).not.toThrow());
  });
});
