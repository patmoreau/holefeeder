import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useAuth } from '@/shared/auth/core/use-auth';
import { BrandBackground } from '@/shared/presentation/components/BrandBackground';
import { BrandTile } from '@/shared/presentation/components/BrandTile';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppPinnedActions } from '@/shared/presentation/components/native/AppPinnedActions';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { useTheme } from '@/shared/theme/core/use-theme';

const WelcomeScreen = () => {
  const { isLoading, login } = useAuth();
  const { theme } = useTheme();

  const { t } = useTranslation();

  return (
    <BrandBackground testID="welcome-screen">
      <AppNative style={{ flex: 1 }}>
        <AppColumn spacing={8} alignment={'center'}>
          <BrandTile testID="welcome-brand-tile" />
          <AppText variant={'largeTitle'} textStyle={{ color: theme.colors.text }}>
            Holefeeder
          </AppText>
          {isLoading ? (
            <AppLoadingIndicator size="large" />
          ) : (
            <>
              <AppText variant={'title'} textStyle={{ color: theme.colors.text }}>
                {t(tk.auth.welcomeTitle)}
              </AppText>
              <AppText variant={'subtitle'} textStyle={{ color: theme.colors.secondaryText }} modifiers={[AppModifiers.centerText]}>
                {t(tk.auth.welcomeSubtitle)}
              </AppText>
            </>
          )}
        </AppColumn>
      </AppNative>
      {/* Both open the same Auth0 flow; only the page it lands on differs. */}
      {!isLoading && (
        <AppPinnedActions
          primary={{ label: t(tk.auth.createAccountButton), onPress: () => login({ signUp: true }), testID: 'welcome-signup-button' }}
          secondary={{ label: t(tk.auth.signInButton), onPress: () => login(), testID: 'welcome-signin-button' }}
          secondaryStyle="button"
        />
      )}
    </BrandBackground>
  );
};

export default WelcomeScreen;
