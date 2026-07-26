import type { ButtonRole } from '@expo/ui/swift-ui';

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
