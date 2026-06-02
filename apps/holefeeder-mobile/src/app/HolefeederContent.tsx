import { Logger } from '@holefeeder/shared/core';
import { Stack } from 'expo-router';
import { useAuth } from '@/shared/auth/core/use-auth';
import { useQuickActions } from '@/shared/hooks/use-quick-actions';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';

const logger = Logger.create('HolefeederContent');

export const HolefeederContent = () => {
  logger.info('AppContent rendering');
  const { user, isLoading } = useAuth();

  useQuickActions();

  if (isLoading) {
    return <AppLoadingIndicator size="large" />;
  }

  return (
    <Stack screenOptions={{ headerShown: false }}>
      <Stack.Protected guard={!!user}>
        <Stack.Screen name="(app)" />
      </Stack.Protected>
      <Stack.Protected guard={!user}>
        <Stack.Screen name="(auth)" />
      </Stack.Protected>
      <Stack.Screen name="+not-found" />
    </Stack>
  );
};
