import { NativeTabs } from 'expo-router/unstable-native-tabs';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { useLanguage } from '@/shared/language/core/use-language';
import { AppIcons } from '@/shared/presentation/icons';
import { useTheme } from '@/shared/theme/core/use-theme';

const TabsLayout = () => {
  const { t } = useTranslation();
  const { language } = useLanguage();
  const { theme } = useTheme();

  return (
    <NativeTabs key={language} iconColor={theme.colors.tabIconDefault} tintColor={theme.colors.tabIconSelected}>
      <NativeTabs.Trigger name="index">
        <NativeTabs.Trigger.Icon sf={AppIcons.dashboard} />
        <NativeTabs.Trigger.Label>{t(tk.tabs.dashboard)}</NativeTabs.Trigger.Label>
      </NativeTabs.Trigger>
      <NativeTabs.Trigger name="accounts">
        <NativeTabs.Trigger.Icon sf={AppIcons.accounts} />
        <NativeTabs.Trigger.Label>{t(tk.tabs.accounts)}</NativeTabs.Trigger.Label>
      </NativeTabs.Trigger>
      <NativeTabs.Trigger name="settings">
        <NativeTabs.Trigger.Icon sf={AppIcons.settings} />
        <NativeTabs.Trigger.Label>{t(tk.tabs.settings)}</NativeTabs.Trigger.Label>
      </NativeTabs.Trigger>
    </NativeTabs>
  );
};

export default TabsLayout;
