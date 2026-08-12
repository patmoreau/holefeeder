import { Logger } from '@holefeeder/shared/core';
import { Stack } from 'expo-router';
import { ApiConfig } from '@/shared/api/api-config';
import { useAuth } from '@/shared/auth/core/use-auth';
import { useQuickActions } from '@/shared/hooks/use-quick-actions';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { RegistrationStatuses } from '@/user-registration/core/registration-status';
import { useRegistrationStatus } from '@/user-registration/presentation/core/use-registration-status';
import { RegistrationError } from '@/user-registration/presentation/RegistrationError';

const logger = Logger.create('HolefeederContent');

export const HolefeederContent = ({ apiConfig }: { apiConfig: ApiConfig }) => {
  logger.info('AppContent rendering');
  const { user, isLoading } = useAuth();
  const { status, recheck } = useRegistrationStatus(apiConfig);

  useQuickActions();

  if (isLoading) {
    return <AppLoadingIndicator size="large" />;
  }

  // Signed in, but we do not yet know which side of the gate they belong on. Showing
  // either group now would flash the wrong one.
  if (user && status.isLoading) {
    return <AppLoadingIndicator size="large" />;
  }

  if (user && status.isFailure) {
    return <RegistrationError onRetry={recheck} />;
  }

  const isRegistered = status.isSuccess && status.value === RegistrationStatuses.registered;
  const isNotRegistered = status.isSuccess && status.value === RegistrationStatuses.notRegistered;

  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Protected guard={!!user && isRegistered}>
        <Stack.Screen name="(app)" />
      </Stack.Protected>
      <Stack.Protected guard={!!user && isNotRegistered}>
        <Stack.Screen name="(onboarding)" />
      </Stack.Protected>
      <Stack.Protected guard={!user}>
        <Stack.Screen name="(auth)" />
      </Stack.Protected>
      <Stack.Screen name="+not-found" />
    </Stack>
  );
};
