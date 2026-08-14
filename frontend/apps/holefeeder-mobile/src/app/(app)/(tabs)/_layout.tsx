import { router } from 'expo-router';
import { NativeTabs } from 'expo-router/unstable-native-tabs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useLanguage } from '@/shared/language/core/use-language';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppToolbar } from '@/shared/presentation/components/native/AppToolbar';
import { AppToolbarButton } from '@/shared/presentation/components/native/AppToolbarButton';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useTheme } from '@/shared/theme/core/use-theme';

const TabsLayout = () => {
  const { t } = useTranslation();
  const { language } = useLanguage();
  const { theme } = useTheme();

  return (
    <>
      <AppToolbar placement="right">
        <AppToolbarButton
          icon={AppIcon.select(AppIconMap.purchase)}
          testID="purchase"
          accessibilityLabel={t(tk.purchase.title)}
          onPress={() => router.push('/(app)/Purchase')}
        />
      </AppToolbar>
      <NativeTabs key={language} iconColor={theme.colors.tabIconDefault} tintColor={theme.colors.tabIconSelected}>
        <NativeTabs.Trigger name="index">
          <NativeTabs.Trigger.Icon sf={AppIconMap.dashboard.ios} />
          <NativeTabs.Trigger.Label>{t(tk.tabs.dashboard)}</NativeTabs.Trigger.Label>
        </NativeTabs.Trigger>
        <NativeTabs.Trigger name="statistics">
          <NativeTabs.Trigger.Icon sf={AppIconMap.insights.ios} />
          <NativeTabs.Trigger.Label>{t(tk.tabs.insights)}</NativeTabs.Trigger.Label>
        </NativeTabs.Trigger>
        <NativeTabs.Trigger name="settings">
          <NativeTabs.Trigger.Icon sf={AppIconMap.settings.ios} />
          <NativeTabs.Trigger.Label>{t(tk.tabs.settings)}</NativeTabs.Trigger.Label>
        </NativeTabs.Trigger>
      </NativeTabs>
    </>
  );
};

export default TabsLayout;
