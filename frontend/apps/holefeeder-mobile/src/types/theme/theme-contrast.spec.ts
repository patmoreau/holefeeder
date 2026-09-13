import { darkTheme } from './dark';
import { lightTheme } from './light';
import { Theme } from './theme';

const channels = (color: string) => [1, 3, 5].map((i) => parseInt(color.slice(i, i + 2), 16));

const linearize = (channel: number) => {
  const value = channel / 255;
  return value <= 0.04045 ? value / 12.92 : ((value + 0.055) / 1.055) ** 2.4;
};

const luminance = (color: string) => {
  const [r, g, b] = channels(color).map(linearize);
  return 0.2126 * r + 0.7152 * g + 0.0722 * b;
};

const contrastRatio = (foreground: string, background: string) => {
  const [lighter, darker] = [luminance(foreground), luminance(background)].sort((a, b) => b - a);
  return (lighter + 0.05) / (darker + 0.05);
};

// WCAG 2.1 AA for text below 18pt regular.
const AA_NORMAL_TEXT = 4.5;

describe('theme contrast', () => {
  const themes: [string, Theme][] = [
    ['light', lightTheme],
    ['dark', darkTheme],
  ];

  describe.each(themes)('%s theme', (_name, theme) => {
    const surfaces: [string, string][] = [
      ['background', theme.colors.background],
      ['secondaryBackground', theme.colors.secondaryBackground],
    ];

    it.each(surfaces)('should render primary text on %s at AA contrast', (_surface, background) => {
      expect(contrastRatio(theme.colors.text, background)).toBeGreaterThanOrEqual(AA_NORMAL_TEXT);
    });

    it.each(surfaces)('should render secondary text on %s at AA contrast', (_surface, background) => {
      expect(contrastRatio(theme.colors.secondaryText, background)).toBeGreaterThanOrEqual(AA_NORMAL_TEXT);
    });
  });
});
