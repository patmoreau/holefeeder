import { Icon } from '@expo/ui';
import { router, Stack } from 'expo-router';
import { NativeTabs } from 'expo-router/unstable-native-tabs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useLanguage } from '@/shared/language/core/use-language';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
import { useTheme } from '@/shared/theme/core/use-theme';

const TabsLayout = () => {
  const { t } = useTranslation();
  const { language } = useLanguage();
  const { theme } = useTheme();

  return (
    <>
      <Stack.Toolbar placement="right">
        <Stack.Toolbar.Button icon={Icon.select(AppIconMap.purchase)} onPress={() => router.push('/(app)/Purchase')} />
      </Stack.Toolbar>
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
