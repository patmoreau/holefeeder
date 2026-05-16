import React from 'react';
import { useTranslation } from 'react-i18next';
import { tk } from '@/i18n/translations';
import { SettingsContent } from '@/settings/presentation/SettingsContent';
import { IconSymbol } from '@/shared/presentation/components/IconSymbol';
import { AppIcons } from '@/shared/presentation/icons';
import { ParallaxScrollView } from '@/shared/presentation/ParallaxScrollView';
import { ScreenTitle } from '@/shared/presentation/ScreenTitle';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';

const createStyles = () => ({
  content: {
    flex: 1,
    overflow: 'hidden' as const,
  },
  container: {
    flex: 1,
    minHeight: '100%' as const,
    backgroundColor: 'red' as const,
  },
  headerImage: {
    color: '#808080' as const,
    bottom: -90,
    left: -35,
    position: 'absolute' as const,
  },
});

const SettingsScreen = () => {
  const { t } = useTranslation();
  const styles = useStyles(createStyles);
  const { theme } = useTheme();

  return (
    <ParallaxScrollView
      style={styles.content}
      headerBackgroundColor={theme.colors.settings}
      headerImage={<IconSymbol size={310} color="#808080" name={AppIcons.settings} style={styles.headerImage} />}
    >
      <ScreenTitle title={t(tk.settings.title)} />
      <SettingsContent />
    </ParallaxScrollView>
  );
};

export default SettingsScreen;
