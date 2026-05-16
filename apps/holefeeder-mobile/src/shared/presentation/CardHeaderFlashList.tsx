import { useHeaderHeight } from '@react-navigation/elements';
import { FlashListProps } from '@shopify/flash-list';
import type { ReactNode } from 'react';
import { useState } from 'react';
import { RefreshControl, View } from 'react-native';
import Reanimated, { Extrapolation, interpolate, useAnimatedScrollHandler, useAnimatedStyle, useSharedValue } from 'react-native-reanimated';
import { AppView } from '@/shared/presentation/AppView';
import { AppCardPagedList } from '@/shared/presentation/components/AppCardPagedList';
import { UsePagedWatchResult } from '@/shared/presentation/core/use-paged-watch';
import { useStyles } from '@/shared/theme/core/use-styles';
import { useTheme } from '@/shared/theme/core/use-theme';
import { spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

const DEFAULT_HEADER_HEIGHT = 300;

type Props<T> = {
  largeCard: ReactNode;
  smallCard: ReactNode;
  headerBackgroundColor?: string;
  pagedResult: UsePagedWatchResult<T>;
  renderItem: NonNullable<FlashListProps<T>['renderItem']>;
  keyExtractor?: FlashListProps<T>['keyExtractor'];
  ListFooterComponent?: FlashListProps<T>['ListFooterComponent'];
  ItemSeparatorComponent?: FlashListProps<T>['ItemSeparatorComponent'];
  onRefresh?: () => void;
  refreshing?: boolean;
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
  contentContainer: {
    paddingHorizontal: spacing.sm,
    paddingBottom: spacing.sm,
    backgroundColor: theme.colors.background,
  },
});

export const CardHeaderFlashList = <T,>({
  largeCard,
  smallCard,
  headerBackgroundColor = '#007AFF',
  pagedResult,
  renderItem,
  keyExtractor,
  ListFooterComponent,
  ItemSeparatorComponent,
  onRefresh,
  refreshing = false,
}: Props<T>) => {
  const styles = useStyles(createStyles);
  const { theme } = useTheme();
  const [largeCardHeight, setLargeCardHeight] = useState(0);

  const HEADER_MIN_HEIGHT = useHeaderHeight();
  const HEADER_MAX_HEIGHT = largeCardHeight > 0 ? largeCardHeight + HEADER_MIN_HEIGHT : DEFAULT_HEADER_HEIGHT;
  const HEADER_SCROLL_DISTANCE = HEADER_MAX_HEIGHT - HEADER_MIN_HEIGHT;

  const scrollOffset = useSharedValue(0);
  const scrollHandler = useAnimatedScrollHandler({
    onScroll: (event) => {
      scrollOffset.value = event.contentOffset.y;
    },
  });

  const headerAnimatedStyle = useAnimatedStyle(() => ({
    height: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [HEADER_MAX_HEIGHT, HEADER_MIN_HEIGHT], Extrapolation.CLAMP),
    borderBottomLeftRadius: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.5], [16, 0], Extrapolation.CLAMP),
    borderBottomRightRadius: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.5], [16, 0], Extrapolation.CLAMP),
    paddingHorizontal: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [20, 0], Extrapolation.CLAMP),
  }));

  const largeCardAnimatedStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 0.7], [1, 0], Extrapolation.CLAMP),
    transform: [
      {
        translateY: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [0, -30], Extrapolation.CLAMP),
      },
    ],
  }));

  const smallCardAnimatedStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [HEADER_SCROLL_DISTANCE * 0.5, HEADER_SCROLL_DISTANCE * 1.2], [0, 1], Extrapolation.CLAMP),
    transform: [
      {
        translateY: interpolate(scrollOffset.value, [0, HEADER_SCROLL_DISTANCE * 1.2], [20, 0], Extrapolation.CLAMP),
      },
    ],
  }));

  return (
    <AppView style={styles.container}>
      <Reanimated.View style={[styles.header, { backgroundColor: headerBackgroundColor }, headerAnimatedStyle]}>
        <Reanimated.View
          style={[styles.largeCardContainer, largeCardAnimatedStyle]}
          onLayout={(event) => {
            const { height } = event.nativeEvent.layout;
            if (height > 0 && height !== largeCardHeight) {
              setLargeCardHeight(height);
            }
          }}
        >
          {largeCard}
        </Reanimated.View>
        <Reanimated.View style={[styles.smallCardContainer, smallCardAnimatedStyle]}>{smallCard}</Reanimated.View>
      </Reanimated.View>

      <AppCardPagedList
        scrollable="vertical"
        pagedResult={pagedResult}
        renderItem={renderItem}
        keyExtractor={keyExtractor}
        ListHeaderComponent={<View style={{ height: HEADER_MAX_HEIGHT }} />}
        ListFooterComponent={ListFooterComponent}
        ItemSeparatorComponent={ItemSeparatorComponent}
        contentContainerStyle={styles.contentContainer}
        onScroll={scrollHandler}
        scrollEventThrottle={8}
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
      />
    </AppView>
  );
};
