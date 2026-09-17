import { spacing } from '@/types/theme/design-tokens';
import { collapsingHeaderHeights, smallCardInsets, TOOLBAR_BUTTON_CLEARANCE } from './collapsing-header-heights';

const WINDOW = 900;
const INSET = 100;

describe('collapsingHeaderHeights', () => {
  describe('before the card has been measured', () => {
    it('should fall back to a third of the window', () => {
      const { fullHeight } = collapsingHeaderHeights({ cardHeight: 0, windowHeight: WINDOW, insetTop: INSET });

      expect(fullHeight).toBe(WINDOW / 3);
    });
  });

  describe('once the card has been measured', () => {
    it('should take the height the card reported', () => {
      const { fullHeight } = collapsingHeaderHeights({ cardHeight: 260, windowHeight: WINDOW, insetTop: INSET });

      expect(fullHeight).toBe(260);
    });

    it('should cap at half the window, so large text cannot fill the screen', () => {
      const { fullHeight } = collapsingHeaderHeights({ cardHeight: 800, windowHeight: WINDOW, insetTop: INSET });

      expect(fullHeight).toBe(WINDOW / 2);
    });
  });

  describe('travel', () => {
    it('should be the distance between the full and collapsed heights', () => {
      const { travel } = collapsingHeaderHeights({ cardHeight: 260, windowHeight: WINDOW, insetTop: INSET });

      expect(travel).toBe(260 - INSET);
    });

    it('should never be negative when the card is shorter than the toolbar area', () => {
      const { travel } = collapsingHeaderHeights({ cardHeight: 40, windowHeight: WINDOW, insetTop: INSET });

      expect(travel).toBe(0);
    });
  });

  describe('the list spacer', () => {
    it('should be the whole header height, since the list applies no inset of its own', () => {
      const { spacerHeight } = collapsingHeaderHeights({ cardHeight: 260, windowHeight: WINDOW, insetTop: INSET });

      expect(spacerHeight).toBe(260);
    });

    it('should follow the capped header height rather than the card', () => {
      const { spacerHeight } = collapsingHeaderHeights({ cardHeight: 800, windowHeight: WINDOW, insetTop: INSET });

      expect(spacerHeight).toBe(WINDOW / 2);
    });
  });

  describe('collapsed height', () => {
    it('should be the top inset, which covers the status bar and the toolbar row', () => {
      const { collapsedHeight } = collapsingHeaderHeights({ cardHeight: 260, windowHeight: WINDOW, insetTop: INSET });

      expect(collapsedHeight).toBe(INSET);
    });
  });
});

describe('smallCardInsets', () => {
  it('should clear the trailing toolbar buttons', () => {
    expect(smallCardInsets({ leadingToolbar: false }).paddingRight).toBe(TOOLBAR_BUTTON_CLEARANCE);
  });

  it('should keep the standard margin on the leading side when no button sits there', () => {
    expect(smallCardInsets({ leadingToolbar: false }).paddingLeft).toBe(spacing.lg);
  });

  it('should clear the leading toolbar button when one sits there', () => {
    expect(smallCardInsets({ leadingToolbar: true }).paddingLeft).toBe(TOOLBAR_BUTTON_CLEARANCE);
  });
});
