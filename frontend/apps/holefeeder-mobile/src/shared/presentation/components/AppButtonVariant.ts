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

// SwiftUI-native role. `destructive` unlocks red tint + full-swipe inside
// SwipeActions. Non-destructive variants MUST be `undefined`, not `'default'`:
// the native SwipeActions parser drops action buttons whose role is `'default'`.
export const variantRole: Record<AppButtonVariant, ButtonRole | undefined> = {
  primary: undefined,
  secondary: undefined,
  destructive: 'destructive',
  link: undefined,
};
