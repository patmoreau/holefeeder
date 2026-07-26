import { Button, ButtonProps } from '@expo/ui';
import type { ButtonProps as SwiftUIButtonProps } from '@expo/ui/swift-ui';
import React from 'react';

// Universal button props plus the iOS-only SwiftUI escape hatches (`role`,
// `systemImage`). They are ignored by the Android/universal Button below and
// forwarded to the native SwiftUI Button in ExpoButton.ios.tsx.
export type ExpoButtonProps = ButtonProps & Pick<SwiftUIButtonProps, 'role' | 'systemImage'>;

export const ExpoButton = ({ role: _role, systemImage: _systemImage, ...props }: ExpoButtonProps) => {
  return <Button {...props} />;
};
