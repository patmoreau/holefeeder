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

// A list sitting behind the header already starts this far down the screen, so the spacer that
// keeps its first row clear of the header is that much shorter than the header itself.
export const LIST_TOP_INSET = 125;

export const collapsingHeaderHeights = ({ cardHeight, windowHeight, insetTop }: CollapsingHeaderInput): CollapsingHeaderHeights => {
  // The card's own height, so the header follows the text size, capped so the largest
  // accessibility sizes cannot grow it over the whole screen.
  const fullHeight = Math.min(cardHeight || windowHeight / 3, windowHeight / 2);

  return {
    fullHeight,
    collapsedHeight: insetTop,
    travel: Math.max(0, fullHeight - insetTop),
    spacerHeight: Math.max(0, fullHeight - LIST_TOP_INSET),
  };
};
