import { router } from 'expo-router';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { View } from 'react-native';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';
import { AppView } from '@/shared/presentation/AppView';
import { AuthButton } from '@/shared/presentation/AuthButton';
import { AppButton } from '@/shared/presentation/components/AppButton';
import { AppText } from '@/shared/presentation/components/AppText';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { AppIcons } from '@/shared/presentation/icons';
import { useStyles } from '@/shared/theme/core/use-styles';
import { GlobalStyles } from '@/types/theme/global-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme, global: typeof GlobalStyles) => ({
  centered: {
    ...theme.styles.containers.center,
    ...global.gapMedium,
  },
});

const LoginScreen = () => {
  const { isLoading } = useAuth();

  const { t } = useTranslation();
  const styles = useStyles(createStyles);

  if (isLoading) {
    return (
      <AppView style={styles.centered}>
        <LoadingIndicator size="large" />
      </AppView>
    );
  }

  return (
    <AppView style={styles.centered}>
      <AppText variant={'title'}>{t(tk.auth.loginTitle)}</AppText>
      <AppText variant={'subtitle'}>{t(tk.auth.loginSubtitle)}</AppText>
      <AuthButton />
      <View style={{ paddingTop: 16 }}>
        <AppButton icon={AppIcons.warning} onPress={() => router.push('/test')} />
      </View>
    </AppView>
  );
};

export default LoginScreen;
