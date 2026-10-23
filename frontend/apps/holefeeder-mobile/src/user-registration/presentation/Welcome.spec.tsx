import { fireEvent, render, screen } from '@testing-library/react-native';
import React from 'react';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';
import WelcomeScreen from '@/user-registration/presentation/Welcome';

const mockLogin = jest.fn();
let mockIsLoading = false;

jest.mock('react-native-safe-area-context', () => ({
  useSafeAreaInsets: () => ({ top: 0, bottom: 34, left: 0, right: 0 }),
}));

jest.mock('@/shared/auth/core/use-auth', () => ({
  useAuth: () => ({ isLoading: mockIsLoading, login: mockLogin }),
}));

const renderScreen = () =>
  render(
    <ThemeProviderForTest overrides={aLightThemeState()}>
      <WelcomeScreen />
    </ThemeProviderForTest>
  );

describe('WelcomeScreen', () => {
  beforeEach(() => {
    mockIsLoading = false;
    mockLogin.mockClear();
  });

  it('shows the brand tile and the app name', async () => {
    await renderScreen();

    expect(screen.getByTestId('welcome-brand-tile')).toBeOnTheScreen();
    expect(screen.getByText('Holefeeder')).toBeOnTheScreen();
  });

  it('opens sign-up from create an account', async () => {
    await renderScreen();

    fireEvent.press(screen.getByText('auth.createAccountButton'));

    expect(mockLogin).toHaveBeenCalledWith({ signUp: true });
  });

  it('opens sign-in from sign in', async () => {
    await renderScreen();

    fireEvent.press(screen.getByText('auth.signInButton'));

    expect(mockLogin).toHaveBeenCalledWith();
  });

  it('shows the brand tile while loading', async () => {
    mockIsLoading = true;

    await renderScreen();

    expect(screen.getByTestId('welcome-brand-tile')).toBeOnTheScreen();
    expect(screen.queryByText('auth.createAccountButton')).not.toBeOnTheScreen();
  });
});
