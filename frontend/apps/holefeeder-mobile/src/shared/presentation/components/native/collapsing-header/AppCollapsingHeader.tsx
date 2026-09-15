import type { ReactNode } from 'react';
import { useWindowDimensions, View } from 'react-native';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { AppReact } from '@/shared/presentation/components/native/AppReact';
import { AppRectangle } from '@/shared/presentation/components/native/AppRectangle';
import { AppZStack } from '@/shared/presentation/components/native/AppZStack';
import { spacing } from '@/types/theme/design-tokens';
import { TOOLBAR_ROW_HEIGHT } from './collapsing-header-heights';
import type { CollapsingHeader } from './use-collapsing-header';

const styles = {
  host: {
    position: 'absolute' as const,
    top: 0,
    left: 0,
    right: 0,
    zIndex: 1000,
  },
  smallCard: {
    paddingLeft: spacing.lg,
    // Clear of the toolbar buttons on the right.
    paddingRight: 72,
    height: TOOLBAR_ROW_HEIGHT,
    justifyContent: 'center' as const,
  },
};

export type AppCollapsingHeaderProps = {
  header: CollapsingHeader;
  /** Shown at rest; slides up and fades out as the list scrolls. */
  children: ReactNode;
  /** Takes its place on the toolbar row once collapsed. */
  small?: ReactNode;
};

export const AppCollapsingHeader = ({ header, children, small }: AppCollapsingHeaderProps) => {
  const { width } = useWindowDimensions();

  return (
    <AppNative style={[styles.host, { height: header.fullHeight }]} pointerEvents="none" ignoreSafeArea="all">
      <AppZStack alignment="topLeading">
        <AppRectangle modifiers={header.barModifiers} />
        <AppZStack alignment="topLeading" modifiers={header.largeCardModifiers}>
          <AppReact matchContents onLayout={header.onCardLayout}>
            <View style={{ width }}>{children}</View>
          </AppReact>
        </AppZStack>
        {small ? (
          <AppZStack alignment="topLeading" modifiers={header.smallCardModifiers}>
            <AppReact matchContents>
              <View style={[styles.smallCard, { width }]}>{small}</View>
            </AppReact>
          </AppZStack>
        ) : null}
      </AppZStack>
    </AppNative>
  );
};
