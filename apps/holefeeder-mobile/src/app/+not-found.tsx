import { router, Stack } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { AppButton } from '@/shared/presentation/components/app/AppButton';
import { AppColumn } from '@/shared/presentation/components/app/AppColumn';
import { AppHost } from '@/shared/presentation/components/app/AppHost';
import { AppText } from '@/shared/presentation/components/app/AppText';
import { useStyles } from '@/shared/theme/core/use-styles';
import { GlobalStyles } from '@/types/theme/global-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  heading: {
    color: theme.colors.primary,
    ...GlobalStyles.textCenter,
    ...GlobalStyles.py16,
  },
  subtitle: {
    ...theme.typography.subtitle,
    color: theme.colors.secondaryText,
    ...GlobalStyles.textCenter,
  },
});

export const NotFoundScreen = () => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);

  return (
    <>
      <Stack.Screen
        options={{
          headerShown: true,
          title: t(tk.notFound.title),
          headerTitleStyle: styles.subtitle,
          headerTransparent: true,
          headerBackButtonMenuEnabled: false,
          headerBackButtonDisplayMode: 'minimal',
        }}
      />
      <AppHost style={{ flex: 1 }}>
        <AppColumn spacing={12} alignment={'center'}>
          <AppText variant="title" textStyle={styles.heading}>
            {t(tk.notFound.description)}
          </AppText>
          <AppButton label={t(tk.notFound.goBack)} variant="primary" onPress={() => router.push({ pathname: '/' })} />
        </AppColumn>
      </AppHost>
    </>
  );
};
