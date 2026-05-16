import { Stack } from 'expo-router';
import { useAuth } from '@/shared/auth/core/use-auth';
import { Logger } from '@/shared/core/logger/logger';
import { useQuickActions } from '@/shared/hooks/use-quick-actions';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useTheme } from '@/shared/theme/core/use-theme';

const logger = Logger.create('HolefeederContent');

const HolefeederContent = () => {
  logger.info('AppContent rendering');
  const { user, isLoading } = useAuth();

  const { theme } = useTheme();

  useQuickActions();

  if (isLoading) {
    return <LoadingIndicator size="large" />;
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
      <Stack.Screen
        name="test"
        options={{
          presentation: 'modal',
          headerTransparent: true,
          headerTintColor: theme.colors.tint,
        }}
      />
    </Stack>
  );
};

export default HolefeederContent;
