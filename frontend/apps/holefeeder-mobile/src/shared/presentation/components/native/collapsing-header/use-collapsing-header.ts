import { useEffect, useId, useState } from 'react';
import { useWindowDimensions, type LayoutChangeEvent } from 'react-native';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { discardCollapsingHeaderStore } from '@/modules/app-modifiers';
import { AppModifiers } from '@/shared/presentation/components/native/AppModifiers';
import { useTheme } from '@/shared/theme/core/use-theme';
import { collapsingHeaderHeights, TOOLBAR_ROW_HEIGHT, TOOLBAR_ROW_INSET } from './collapsing-header-heights';

// Where the two cards hand over. They overlap, otherwise the large one is gone before the small
// one arrives and the header reads as empty part way through the scroll.
const LARGE_FADE_END = 0.5;
const SMALL_FADE_START = 0.3;
const SMALL_FADE_END = 0.6;

export type CollapsingHeader = ReturnType<typeof useCollapsingHeader>;

/**
 * Drives a header that shrinks to the toolbar area as a list scrolls beneath it.
 *
 * The scroll offset never reaches JavaScript: `listModifiers` hands it to the SwiftUI side, which
 * interpolates the header on the main thread alongside the list itself. Pass `listModifiers` to the
 * scrolling `AppList`, render `spacerHeight` as its first row, and give the rest to
 * `AppCollapsingHeader`.
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
  const id = useId();
  const { theme } = useTheme();
  const { height: windowHeight } = useWindowDimensions();
  const insets = useSafeAreaInsets();
  const [cardHeight, setCardHeight] = useState(0);
  const [smallCardHeight, setSmallCardHeight] = useState(0);

  useEffect(() => () => discardCollapsingHeaderStore(id), [id]);

  const { fullHeight, collapsedHeight, travel, spacerHeight } = collapsingHeaderHeights({
    cardHeight,
    windowHeight,
    insetTop: collapsedHeightOverride ?? insets.top,
  });

  return {
    fullHeight,
    collapsedHeight,
    spacerHeight,
    barModifiers: [
      AppModifiers.collapsingHeaderBar({
        id,
        fullHeight,
        collapsedHeight,
        backgroundColor: theme.colors.primary,
      }),
    ],
    largeCardModifiers: [AppModifiers.collapsingHeaderLargeCard({ id, travel, fadeEnd: LARGE_FADE_END })],
    smallCardModifiers: [
      AppModifiers.collapsingHeaderSmallCard({
        id,
        fullHeight,
        collapsedHeight,
        rowInset: TOOLBAR_ROW_INSET,
        rowHeight: Math.max(smallCardHeight, TOOLBAR_ROW_HEIGHT),
        fadeStart: SMALL_FADE_START,
        fadeEnd: SMALL_FADE_END,
      }),
    ],
    onCardLayout: (event: LayoutChangeEvent) => setCardHeight(event.nativeEvent.layout.height),
    onSmallCardLayout: (event: LayoutChangeEvent) => setSmallCardHeight(event.nativeEvent.layout.height),
    // Without this the list takes a top content inset from the transparent navigation bar, and
    // that inset is only applied the first time a screen is presented. The spacer row then has to
    // be the whole header height, and where the first row lands stops depending on it.
    listModifiers: [AppModifiers.collapsingHeaderSource(id), AppModifiers.ignoreTopSafeArea],
  };
};
