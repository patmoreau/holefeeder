// Mirrors the SwiftUI Button role (kept local so this module has no @expo/ui
// dependency); structurally assignable to swift-ui's ButtonRole.
export type ButtonRole = 'default' | 'cancel' | 'destructive';

export const AppButtonVariant = {
  primary: 'primary',
  secondary: 'secondary',
  destructive: 'destructive',
  link: 'link',
} as const;

export type AppButtonVariant = (typeof AppButtonVariant)[keyof typeof AppButtonVariant];

// SwiftUI-native role. `destructive` unlocks red tint + full-swipe inside SwipeActions.
export const variantRole: Record<AppButtonVariant, ButtonRole> = {
  primary: 'default',
  secondary: 'default',
  destructive: 'destructive',
  link: 'default',
};
