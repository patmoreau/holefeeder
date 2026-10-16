import { Result } from '@holefeeder/shared/core';
import { Stack } from 'expo-router';
import { renderRouter, screen } from 'expo-router/testing-library';
import { Text } from 'react-native';
import { HolefeederContent } from '@/app/HolefeederContent';
import { useAuth } from '@/shared/auth/core/use-auth';
import { RegistrationStatuses, type RegistrationStatus } from '@/user-registration/core/registration-status';
import { useRegistration } from '@/user-registration/presentation/RegistrationProvider';

jest.mock('@/shared/auth/core/use-auth');
jest.mock('@/user-registration/presentation/RegistrationProvider');
jest.mock('@/shared/hooks/use-quick-actions', () => ({ useQuickActions: jest.fn() }));

const signedIn = (status: RegistrationStatus) => {
  (useAuth as jest.Mock).mockReturnValue({ user: { sub: 'auth0|user' }, isLoading: false });
  (useRegistration as jest.Mock).mockReturnValue({ status: Result.success(status), recheck: jest.fn() });
};

const signedOut = () => {
  (useAuth as jest.Mock).mockReturnValue({ user: undefined, isLoading: false });
  (useRegistration as jest.Mock).mockReturnValue({ status: Result.loading(), recheck: jest.fn() });
};

const routes = {
  _layout: () => <HolefeederContent />,
  '(app)/_layout': () => <Stack />,
  '(app)/index': () => <Text>Dashboard</Text>,
  '(onboarding)/_layout': () => <Stack />,
  '(onboarding)/Registering': () => <Text>Registering</Text>,
  '(auth)/_layout': () => <Stack />,
  '(auth)/Welcome': () => <Text>Welcome</Text>,
  '+not-found': () => <Text>Not found</Text>,
};

describe('HolefeederContent', () => {
  it('shows the welcome screen at launch when signed out', async () => {
    signedOut();

    await renderRouter(routes, { initialUrl: '/' });

    expect(screen.getByText('Welcome')).toBeVisible();
  });

  it('shows onboarding at launch when signed in but not registered', async () => {
    signedIn(RegistrationStatuses.notRegistered);

    await renderRouter(routes, { initialUrl: '/' });

    expect(screen.getByText('Registering')).toBeVisible();
  });

  it('shows the dashboard at launch when signed in and registered', async () => {
    signedIn(RegistrationStatuses.registered);

    await renderRouter(routes, { initialUrl: '/' });

    expect(screen.getByText('Dashboard')).toBeVisible();
  });
});
