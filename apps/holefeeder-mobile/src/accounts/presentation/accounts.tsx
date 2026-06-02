import { useState } from 'react';
import { StyleSheet } from 'react-native';
import { AppCollapsible } from '@/shared/presentation/components/native/AppCollapsible';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppIcon } from '@/shared/presentation/components/native/AppIcon';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppText } from '@/shared/presentation/components/native/AppText';
import { AppIconMap } from '@/shared/presentation/core/app-icon-map';
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
        <AppNative>
          <AppIcon size={310} color="#808080" name={AppIconMap.accounts} />
        </AppNative>
      }
    >
      <ScreenTitle title={'Explore'} />
      <AppNative style={{ flex: 1 }}>
        <AppColumn spacing={8} style={{ padding: 16 }}>
          <AppCollapsible label="Section a" isOpen={openSection === 'a'} onOpenChange={(open) => setOpenSection(open ? 'a' : null)}>
            <AppText>Coucou</AppText>
          </AppCollapsible>
        </AppColumn>
      </AppNative>
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
