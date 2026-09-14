import React, { useState } from 'react';
import { useWindowDimensions, View } from 'react-native';
import Animated, { Extrapolation, interpolate, useAnimatedStyle, useSharedValue } from 'react-native-reanimated';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { AppColumn } from '@/shared/presentation/components/native/AppColumn';
import { AppList } from '@/shared/presentation/components/native/AppList';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { AppNative } from '@/shared/presentation/components/native/AppNative';
import { useStyles } from '@/shared/theme/core/use-styles';
import { NO_SUMMARY } from '@/summary/core/watch-summary/watch-summary-use-case';
import { useSummary } from '@/summary/presentation/core/use-summary';
import { SpendingHeaderSmallCard } from '@/summary/presentation/SpendingHeaderSmallCard';
import { Theme } from '@/types/theme/theme';
import { CategorySpendingList } from './CategorySpendingList';
import { CombinedInsightToggle } from './CombinedInsightToggle';
import { CombinedSpendingList } from './CombinedSpendingList';
import { useCombinedInsight } from './core/use-combined-insight';
import { InsightsPeriodHeader } from './InsightsPeriodHeader';
import { TagSpendingList } from './TagSpendingList';

// Matches the dashboard: the toolbar row the collapsed header leaves visible, and the offset that
// lines its contents up with the toolbar buttons rather than the header's bottom edge.
const TOOLBAR_ROW_HEIGHT = 44;
const TOOLBAR_ROW_INSET = 9;
// The list already starts this far down the screen, under the header, so the spacer that keeps
// the first row clear of the header is that much shorter than the header itself.
const LIST_TOP_INSET = 125;

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
    left: 16,
    right: 72,
    minHeight: TOOLBAR_ROW_HEIGHT,
    justifyContent: 'center' as const,
  },
});

export default function InsightsScreen() {
  const { combined } = useCombinedInsight();
  const summaryResult = useSummary();
  const styles = useStyles(createStyles);
  const { height } = useWindowDimensions();
  const insets = useSafeAreaInsets();

  const summary = summaryResult.isSuccess ? summaryResult.value : NO_SUMMARY;

  // As on the dashboard: the header is as tall as its content needs, capped so the largest
  // text sizes cannot grow it over the whole screen.
  const [cardHeight, setCardHeight] = useState(0);
  const fullHeight = Math.min(cardHeight || height / 3, height / 2);
  const collapsedHeight = insets.top;

  const scrollOffset = useSharedValue(0);
  const travel = fullHeight - collapsedHeight;
  const headerStyle = useAnimatedStyle(() => ({
    height: interpolate(scrollOffset.value, [0, travel], [fullHeight, collapsedHeight], Extrapolation.CLAMP),
  }));
  const smallCardStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [travel * 0.3, travel * 0.6], [0, 1], Extrapolation.CLAMP),
  }));
  const cardStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [0, travel * 0.5], [1, 0], Extrapolation.CLAMP),
    transform: [{ translateY: -interpolate(scrollOffset.value, [0, travel], [0, travel], Extrapolation.CLAMP) }],
  }));

  return (
    <View style={{ flex: 1 }} testID="insights-screen">
      <Animated.View style={[styles.header, headerStyle]}>
        <Animated.View style={cardStyle} onLayout={(event) => setCardHeight(event.nativeEvent.layout.height)}>
          <InsightsPeriodHeader />
        </Animated.View>
        <Animated.View style={[styles.smallCardContainer, smallCardStyle]} pointerEvents="none">
          <SpendingHeaderSmallCard summary={summary} />
        </Animated.View>
      </Animated.View>

      <View style={{ flex: 1 }}>
        <AppNative style={{ flex: 1 }}>
          <AppList
            inset
            modifiers={[
              AppModifiers.onScrollOffsetChange((offsetY) => {
                scrollOffset.value = offsetY;
              }),
            ]}
          >
            <AppColumn style={{ paddingTop: fullHeight - LIST_TOP_INSET }} />
            <CombinedInsightToggle />
            {combined ? (
              <CombinedSpendingList />
            ) : (
              <>
                <CategorySpendingList />
                <TagSpendingList />
              </>
            )}
          </AppList>
        </AppNative>
      </View>
    </View>
  );
}
