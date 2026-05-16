import { fireEvent, render, screen } from '@testing-library/react-native';
import { AppButton } from '@/shared/presentation/components/AppButton';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { useTheme } from '@/shared/theme/core/use-theme';

jest.mock('@/shared/theme/core/use-theme');
const mockUseTheme = jest.mocked(useTheme);

const label = 'button-text';

xdescribe('<AppButton />', () => {
  const mockTheme = aLightThemeState();
  const mockOnPress = jest.fn();

  beforeEach(() => {
    mockUseTheme.mockReturnValue(mockTheme);
    mockOnPress.mockClear();
  });

  it('should render', () => {
    render(<AppButton label={label} accessibilityLabel={label} />);
    screen.debug();
    const button = screen.queryByRole('button', { name: label });

    expect(button).toBeOnTheScreen();
    expect(button).toHaveTextContent(label);
  });

  it('should call onPress when clicked', () => {
    render(<AppButton label={label} accessibilityLabel={label} onPress={mockOnPress} />);

    const button = screen.queryByRole('button', { name: label });

    fireEvent.press(button);
    expect(mockOnPress).toHaveBeenCalled();
  });

  describe('with variant', () => {
    it('should render primary', () => {
      render(<AppButton label={label} accessibilityLabel={label} variant="primary" />);

      const button = screen.getByTestId('button');

      expect(button.props['data-variant']).toBe('glassProminent');
    });

    it('should render secondary', () => {
      render(<AppButton label={label} accessibilityLabel={label} variant="secondary" />);

      const button = screen.getByTestId('button');

      expect(button.props['data-variant']).toBe('default');
    });

    it('should render destructive', () => {
      render(<AppButton label={label} accessibilityLabel={label} variant="destructive" />);

      const button = screen.getByTestId('button');

      expect(button.props['data-role']).toBe('destructive');
    });

    it('should render link', () => {
      render(<AppButton label={label} accessibilityLabel={label} variant="link" />);

      const button = screen.getByTestId('button');

      expect(button.props['data-variant']).toBe('link');
    });
  });
});
