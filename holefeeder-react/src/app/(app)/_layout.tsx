import { Stack } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useTheme } from '@/shared/theme/core/use-theme';

const AppLayout = () => {
  const { t } = useTranslation();
  const { theme } = useTheme();

  return (
    <Stack>
      <Stack.Screen
        name="(tabs)"
        options={{
          headerTitle: '',
          headerTransparent: true,
        }}
      />
      <Stack.Screen
        name="Purchase"
        options={{
          title: t(tk.purchase.title),
          headerTransparent: true,
          headerTintColor: theme.colors.tint,
        }}
      />
      <Stack.Screen
        name="EditAccount"
        options={{
          title: t(tk.accountEdit.title),
          headerTransparent: true,
          headerTintColor: theme.colors.tint,
        }}
      />
      <Stack.Screen
        name="BudgetSettings"
        options={{
          presentation: 'modal',
          title: t(tk.budgetSection.title),
          headerTransparent: true,
          headerTintColor: theme.colors.tint,
        }}
      />
      <Stack.Screen
        name="SyncSettings"
        options={{
          presentation: 'modal',
          title: t(tk.settings.syncSection.title),
          headerTransparent: true,
          headerTintColor: theme.colors.tint,
        }}
      />
      <Stack.Screen
        name="PayUpcoming"
        options={{
          presentation: 'modal',
          title: t(tk.payUpcoming.title),
          headerShown: true,
          headerTintColor: theme.colors.tint,
        }}
      />
      <Stack.Screen
        name="accounts/[id]"
        options={{
          presentation: 'transparentModal',
          title: '',
          headerTransparent: true,
        }}
      />
      <Stack.Screen
        name="flows/[id]"
        options={{
          presentation: 'transparentModal',
          title: '',
          headerTransparent: true,
        }}
      />
    </Stack>
  );
};

export default AppLayout;
