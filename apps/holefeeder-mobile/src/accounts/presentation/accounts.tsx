import { useState } from 'react';
import { StyleSheet } from 'react-native';
import { AppIconMap } from '@/shared/presentation/components/app/app-icon-map';
import { AppCollapsible } from '@/shared/presentation/components/app/AppCollapsible';
import { AppColumn } from '@/shared/presentation/components/app/AppColumn';
import { AppHost } from '@/shared/presentation/components/app/AppHost';
import { AppIcon } from '@/shared/presentation/components/app/AppIcon';
import { AppText } from '@/shared/presentation/components/app/AppText';
import { ParallaxScrollView } from '@/shared/presentation/ParallaxScrollView';
import { ScreenTitle } from '@/shared/presentation/ScreenTitle';
import { useTheme } from '@/shared/theme/core/use-theme';

type Section = 'a' | 'b' | 'c' | null;

export default function AccountsScreen() {
  const { theme } = useTheme();
  const [openSection, setOpenSection] = useState<Section>(null);

  return (
    <ParallaxScrollView
      style={styles.content}
      headerBackgroundColor={theme.colors.primary}
      headerImage={
        <AppHost>
          <AppIcon size={310} color="#808080" name={AppIconMap.accounts} />
        </AppHost>
      }
    >
      <ScreenTitle title={'Explore'} />
      <AppHost style={{ flex: 1 }}>
        <AppColumn spacing={8} style={{ padding: 16 }}>
          <AppCollapsible label="Section a" isOpen={openSection === 'a'} onOpenChange={(open) => setOpenSection(open ? 'a' : null)}>
            <AppText>Coucou</AppText>
          </AppCollapsible>
        </AppColumn>
      </AppHost>
    </ParallaxScrollView>
  );
}

const styles = StyleSheet.create({
  content: {
    flex: 1,
    padding: 32,
    gap: 16,
    overflow: 'hidden',
  },
});
