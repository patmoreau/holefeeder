import { useHeaderHeight } from '@react-navigation/elements';
import type { ReactNode } from 'react';
import { useState } from 'react';
import { NativeScrollEvent, NativeSyntheticEvent, RefreshControl, type ViewProps } from 'react-native';
import Animated, { Extrapolation, interpolate, useAnimatedRef, useAnimatedStyle, useScrollOffset } from 'react-native-reanimated';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { AppView } from '@/shared/presentation/AppView';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { spacing } from '@/types/theme';
import { Theme } from '@/types/theme/theme';

const DEFAULT_HEADER_HEIGHT = 300;

type Props = ViewProps & {
  largeCard: ReactNode;
  smallCard: ReactNode;
  headerBackgroundColor?: string;
  children: ReactNode;
  onRefresh?: () => void;
  refreshing?: boolean;
  onEndReached?: () => void;
  onEndReachedThreshold?: number;
};

const createStyles = (theme: Theme) => ({
  container: {
    flex: 1,
  },
  header: {
    position: 'absolute' as const,
    top: 0,
    left: 0,
    right: 0,
    zIndex: 1000,
    justifyContent: 'flex-end' as const,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 4,
  },
  largeCardContainer: {
    position: 'absolute' as const,
    paddingTop: 20,
    bottom: 20,
    left: 20,
    right: 20,
  },
  smallCardContainer: {
    position: 'absolute' as const,
    bottom: 16,
    left: 16,
    right: 16,
    alignItems: 'flex-start' as const,
  },
  scrollArea: {
    flex: 1,
  },
  content: {
    backgroundColor: theme.colors.background,
    minHeight: '100%' as const,
  },
});

export const CardHeaderScrollView = ({
  largeCard,
  smallCard,
  headerBackgroundColor = '#007AFF',
  children,
  onRefresh,
  refreshing = false,
  onEndReached,
  onEndReachedThreshold = 200,
  ...otherProps
}: Props) => {
  const styles = useStyles(createStyles);
  const { theme } = useTheme();
  const [largeCardHeight, setLargeCardHeight] = useState(0);
  const { bottom } = useSafeAreaInsets();

  const HEADER_MIN_HEIGHT = useHeaderHeight();
  const HEADER_MAX_HEIGHT = largeCardHeight > 0 ? largeCardHeight + HEADER_MIN_HEIGHT : DEFAULT_HEADER_HEIGHT;
  const HEADER_SCROLL_DISTANCE = HEADER_MAX_HEIGHT - HEADER_MIN_HEIGHT;

  const scrollRef = useAnimatedRef<Animated.ScrollView>();
  const scrollOffset = useScrollOffset(scrollRef);

  const handleScrollEnd = ({ nativeEvent }: NativeSyntheticEvent<NativeScrollEvent>) => {
    if (!onEndReached) return;
    const { layoutMeasurement, contentOffset, contentSize } = nativeEvent;
    if (contentOffset.y + layoutMeasurement.height >= contentSize.height - onEndReachedThreshold) {
      onEndReached();
    }
  };

  const headerAnimatedStyle = useAnimatedStyle(() => {
    return {
      height: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [HEADER_MAX_HEIGHT, HEADER_MIN_HEIGHT], Extrapolation.CLAMP),
      borderBottomLeftRadius: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.5], [16, 0], Extrapolation.CLAMP),
      borderBottomRightRadius: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.5], [16, 0], Extrapolation.CLAMP),
      paddingHorizontal: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [20, 0], Extrapolation.CLAMP),
    };
  });

  const largeCardAnimatedStyle = useAnimatedStyle(() => {
    return {
      opacity: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 0.7], [1, 0], Extrapolation.CLAMP),
      transform: [
        {
          translateY: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [0, -30], Extrapolation.CLAMP),
        },
      ],
    };
  });

  const smallCardAnimatedStyle = useAnimatedStyle(() => {
    return {
      opacity: interpolate(scrollOffset.value, [HEADER_SCROLL_DISTANCE * 0.5, HEADER_SCROLL_DISTANCE * 1.2], [0, 1], Extrapolation.CLAMP),
      transform: [
        {
          translateY: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [20, 0], Extrapolation.CLAMP),
        },
      ],
    };
  });

  return (
    <AppView style={styles.container}>
      <Animated.View style={[styles.header, { backgroundColor: headerBackgroundColor }, headerAnimatedStyle]}>
        <Animated.View
          style={[styles.largeCardContainer, largeCardAnimatedStyle]}
          onLayout={(event) => {
            const { height } = event.nativeEvent.layout;
            if (height > 0 && height !== largeCardHeight) {
              setLargeCardHeight(height);
            }
          }}
        >
          {largeCard}
        </Animated.View>
        <Animated.View style={[styles.smallCardContainer, smallCardAnimatedStyle]}>{smallCard}</Animated.View>
      </Animated.View>

      <Animated.ScrollView
        ref={scrollRef}
        style={styles.scrollArea}
        scrollEventThrottle={8}
        removeClippedSubviews={false}
        contentContainerStyle={{ paddingTop: HEADER_MAX_HEIGHT, paddingBottom: bottom + spacing.lg }}
        onMomentumScrollEnd={handleScrollEnd}
        onScrollEndDrag={handleScrollEnd}
        refreshControl={
          onRefresh ? (
            <RefreshControl
              refreshing={refreshing}
              onRefresh={onRefresh}
              progressViewOffset={HEADER_MAX_HEIGHT}
              tintColor={theme.colors.text}
              colors={[theme.colors.text]}
              progressBackgroundColor={theme.colors.secondaryBackground}
            />
          ) : undefined
        }
      >
        <AppView style={[styles.content]} {...otherProps}>
          {children}
        </AppView>
      </Animated.ScrollView>
    </AppView>
  );
};
