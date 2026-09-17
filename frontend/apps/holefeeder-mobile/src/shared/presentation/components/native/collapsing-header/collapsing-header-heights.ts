import { spacing } from '@/types/theme/design-tokens';

export type CollapsingHeaderInput = {
  /** Height the header's card reported, or 0 before it has laid out. */
  cardHeight: number;
  windowHeight: number;
  /** Top safe-area inset, which under a transparent stack header is the toolbar area. */
  insetTop: number;
};

export type CollapsingHeaderHeights = {
  fullHeight: number;
  collapsedHeight: number;
  travel: number;
  spacerHeight: number;
};

// The toolbar row the collapsed header leaves visible, and how far above the header's bottom edge
// its buttons actually sit — measured against the toolbar rather than derived, because the safe
// area reports the whole header rather than that row.
export const TOOLBAR_ROW_HEIGHT = 44;
export const TOOLBAR_ROW_INSET = 9;
export const TOOLBAR_BUTTON_CLEARANCE = 72;

export const smallCardInsets = ({ leadingToolbar }: { leadingToolbar: boolean }) => ({
  paddingLeft: leadingToolbar ? TOOLBAR_BUTTON_CLEARANCE : spacing.lg,
  paddingRight: TOOLBAR_BUTTON_CLEARANCE,
});

export const collapsingHeaderHeights = ({ cardHeight, windowHeight, insetTop }: CollapsingHeaderInput): CollapsingHeaderHeights => {
  // The card's own height, so the header follows the text size, capped so the largest
  // accessibility sizes cannot grow it over the whole screen.
  const fullHeight = Math.min(cardHeight || windowHeight / 3, windowHeight / 2);

  return {
    fullHeight,
    collapsedHeight: insetTop,
    travel: Math.max(0, fullHeight - insetTop),
    spacerHeight: fullHeight,
  };
};
