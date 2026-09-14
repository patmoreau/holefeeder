import { useState } from 'react';
import { useWindowDimensions, type LayoutChangeEvent } from 'react-native';
import { Extrapolation, interpolate, useAnimatedStyle, useSharedValue } from 'react-native-reanimated';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { collapsingHeaderHeights } from './collapsing-header-heights';

// Where the two cards hand over. They overlap, otherwise the large one is gone before the small
// one arrives and the header reads as empty part way through the scroll.
const LARGE_FADE_END = 0.5;
const SMALL_FADE_START = 0.3;
const SMALL_FADE_END = 0.6;

export type CollapsingHeader = ReturnType<typeof useCollapsingHeader>;

/**
 * Drives a header that shrinks to the toolbar area as a list scrolls beneath it.
 *
 * The offset lives in a Reanimated shared value, so scrolling never re-renders the screen. Pass
 * `listModifiers` to the scrolling `AppList`, render `spacerHeight` as its first row, and give the
 * rest to `AppCollapsingHeader`.
 */
export type UseCollapsingHeaderOptions = {
  /**
   * Height the header collapses to. Defaults to the top safe-area inset, which under the tabs'
   * transparent header is the toolbar area. A pushed screen should pass `useHeaderHeight()`
   * instead: there the inset is only the status bar, and the toolbar row would be swallowed.
   */
  collapsedHeight?: number;
};

export const useCollapsingHeader = ({ collapsedHeight: collapsedHeightOverride }: UseCollapsingHeaderOptions = {}) => {
  const { height: windowHeight } = useWindowDimensions();
  const insets = useSafeAreaInsets();
  const [cardHeight, setCardHeight] = useState(0);

  const { fullHeight, collapsedHeight, travel, spacerHeight } = collapsingHeaderHeights({
    cardHeight,
    windowHeight,
    insetTop: collapsedHeightOverride ?? insets.top,
  });

  const scrollOffset = useSharedValue(0);

  const headerStyle = useAnimatedStyle(() => ({
    height: interpolate(scrollOffset.value, [0, travel], [fullHeight, collapsedHeight], Extrapolation.CLAMP),
  }));

  const largeCardStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [0, travel * LARGE_FADE_END], [1, 0], Extrapolation.CLAMP),
    transform: [{ translateY: -interpolate(scrollOffset.value, [0, travel], [0, travel], Extrapolation.CLAMP) }],
  }));

  const smallCardStyle = useAnimatedStyle(() => ({
    opacity: interpolate(scrollOffset.value, [travel * SMALL_FADE_START, travel * SMALL_FADE_END], [0, 1], Extrapolation.CLAMP),
  }));

  return {
    collapsedHeight,
    spacerHeight,
    headerStyle,
    largeCardStyle,
    smallCardStyle,
    onCardLayout: (event: LayoutChangeEvent) => setCardHeight(event.nativeEvent.layout.height),
    listModifiers: [
      AppModifiers.onScrollOffsetChange((offsetY: number) => {
        scrollOffset.value = offsetY;
      }),
    ],
  };
};
