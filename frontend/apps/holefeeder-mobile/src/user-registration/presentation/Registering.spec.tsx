import { Result } from '@holefeeder/shared/core';
import { render, screen } from '@testing-library/react-native';
import React from 'react';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';
import { OnboardingRegistrationState } from '@/user-registration/presentation/core/use-onboarding-registration';
import RegisteringScreen from '@/user-registration/presentation/Registering';

let mockState: OnboardingRegistrationState;

jest.mock('expo-router', () => ({ router: { replace: jest.fn() } }));
jest.mock('@/user-registration/presentation/core/use-onboarding-registration', () => ({
  useOnboardingRegistration: () => mockState,
}));

const renderScreen = () =>
  render(
    <ThemeProviderForTest overrides={aLightThemeState()}>
      <RegisteringScreen />
    </ThemeProviderForTest>
  );

describe('RegisteringScreen', () => {
  it('shows the brand tile while registering', async () => {
    mockState = { progress: Result.loading(), ready: false, retry: jest.fn() };

    await renderScreen();

    expect(screen.getByTestId('onboarding-registering-screen')).toBeOnTheScreen();
    expect(screen.getByTestId('onboarding-registering-brand-tile')).toBeOnTheScreen();
  });

  it('shows the brand tile when registering failed', async () => {
    mockState = { progress: Result.failure(['error']), ready: false, retry: jest.fn() };

    await renderScreen();

    expect(screen.getByTestId('onboarding-registering-failed')).toBeOnTheScreen();
    expect(screen.getByTestId('onboarding-registering-brand-tile')).toBeOnTheScreen();
  });
});
