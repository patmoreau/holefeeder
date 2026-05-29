import { frame } from '@expo/ui/swift-ui/modifiers';

export const AppModifiers = {
  fillWidth: frame({ maxWidth: Infinity }),
} as const;
