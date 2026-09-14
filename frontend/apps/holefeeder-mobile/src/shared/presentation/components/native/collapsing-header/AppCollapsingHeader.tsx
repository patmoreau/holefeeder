import type { ReactNode } from 'react';
import Animated from 'react-native-reanimated';
import { useStyles } from '@/shared/theme/core/use-styles';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';
import type { CollapsingHeader } from './use-collapsing-header';

// The toolbar row the collapsed header leaves visible, and how far above the header's bottom edge
// its buttons actually sit — measured against the toolbar rather than derived, because the safe
// area reports the whole header rather than that row.
const TOOLBAR_ROW_HEIGHT = 44;
const TOOLBAR_ROW_INSET = 9;

const createStyles = (theme: Theme) => ({
  header: {
    position: 'absolute' as const,
    top: 0,
    left: 0,
    right: 0,
    zIndex: 1000,
    overflow: 'hidden' as const,
    backgroundColor: theme.colors.primary,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 4,
  },
  smallCardContainer: {
    position: 'absolute' as const,
    bottom: TOOLBAR_ROW_INSET,
    left: spacing.lg,
    // Clear of the toolbar buttons on the right.
    right: 72,
    minHeight: TOOLBAR_ROW_HEIGHT,
    justifyContent: 'center' as const,
  },
});

export type AppCollapsingHeaderProps = {
  header: CollapsingHeader;
  /** Shown at rest; slides up and fades out as the list scrolls. */
  children: ReactNode;
  /** Takes its place on the toolbar row once collapsed. */
  small?: ReactNode;
};

export const AppCollapsingHeader = ({ header, children, small }: AppCollapsingHeaderProps) => {
  const styles = useStyles(createStyles);

  return (
    <Animated.View style={[styles.header, header.headerStyle]}>
      <Animated.View style={header.largeCardStyle} onLayout={header.onCardLayout}>
        {children}
      </Animated.View>
      {small ? (
        <Animated.View style={[styles.smallCardContainer, header.smallCardStyle]} pointerEvents="none">
          {small}
        </Animated.View>
      ) : null}
    </Animated.View>
  );
};
