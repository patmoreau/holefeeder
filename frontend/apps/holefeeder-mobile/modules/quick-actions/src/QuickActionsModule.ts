import { NativeModule, requireNativeModule } from 'expo';

export type QuickActionParams = {
  href?: string;
};

export type QuickAction = {
  /** Stable identifier, surfaced to the OS as the shortcut item type. */
  id: string;
  title: string;
  subtitle?: string;
  /** SF Symbol name, e.g. `cart.fill.badge.plus`. */
  icon?: string;
  params?: QuickActionParams;
};

type QuickActionsEvents = {
  onQuickAction: (action: QuickAction) => void;
};

declare class QuickActionsNativeModule extends NativeModule<QuickActionsEvents> {
  setItems(items: QuickAction[]): Promise<void>;
  /**
   * The action the app was launched with, or null. Reading it clears it, so a
   * later remount does not navigate again.
   *
   * Deliberately a function rather than a constant: the shortcut only reaches
   * the app when its scene connects, which is after the module registry — and
   * therefore any constant — has already been built.
   */
  getInitialAction(): Promise<QuickAction | null>;
}

export default requireNativeModule<QuickActionsNativeModule>('QuickActions');
