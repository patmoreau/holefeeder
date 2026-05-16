import { render, screen } from '@testing-library/react-native';
import { aLanguageState } from '@/shared/language/__tests__/language-state-for-test';
import { LanguageProviderForTest } from '@/shared/language/__tests__/LanguageProviderForTest';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';

describe('<LoadingIndicator />', () => {
  const themeState = aLightThemeState();
  const languageState = aLanguageState();

  it('should render with default props', () => {
    render(
      <LanguageProviderForTest overrides={languageState}>
        <ThemeProviderForTest overrides={themeState}>
          <LoadingIndicator />
        </ThemeProviderForTest>
      </LanguageProviderForTest>
    );
    const activityIndicator = screen.queryByLabelText('common.loading');

    expect(activityIndicator.props.size).toBe('large');
    expect(activityIndicator.props.color).toBe(themeState.theme.colors.primary);
  });

  describe('with variant', () => {
    it('should render with primary color', () => {
      render(
        <LanguageProviderForTest overrides={languageState}>
          <ThemeProviderForTest overrides={themeState}>
            <LoadingIndicator variant="primary" />
          </ThemeProviderForTest>
        </LanguageProviderForTest>
      );
      const activityIndicator = screen.queryByLabelText('common.loading');

      expect(activityIndicator.props.color).toBe(themeState.theme.colors.primary);
    });

    it('should render with secondary color', () => {
      render(
        <LanguageProviderForTest overrides={languageState}>
          <ThemeProviderForTest overrides={themeState}>
            <LoadingIndicator variant="secondary" />
          </ThemeProviderForTest>
        </LanguageProviderForTest>
      );
      const activityIndicator = screen.queryByLabelText('common.loading');

      expect(activityIndicator.props.color).toBe(themeState.theme.colors.secondary);
    });
  });

  describe('with size', () => {
    it('should render small', () => {
      render(
        <LanguageProviderForTest overrides={languageState}>
          <ThemeProviderForTest overrides={themeState}>
            <LoadingIndicator size="small" />
          </ThemeProviderForTest>
        </LanguageProviderForTest>
      );
      const activityIndicator = screen.queryByLabelText('common.loading');

      expect(activityIndicator.props.size).toBe('small');
    });

    it('should render large', () => {
      render(
        <LanguageProviderForTest overrides={languageState}>
          <ThemeProviderForTest overrides={themeState}>
            <LoadingIndicator size="large" />
          </ThemeProviderForTest>
        </LanguageProviderForTest>
      );
      const activityIndicator = screen.queryByLabelText('common.loading');

      expect(activityIndicator.props.size).toBe('large');
    });
  });
});
