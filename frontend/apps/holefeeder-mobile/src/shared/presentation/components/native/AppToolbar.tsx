import { Stack } from 'expo-router';
import React from 'react';
import { View } from 'react-native';
import { E2eConfig } from '@/shared/auth/core/e2e-config';

export type AppToolbarPlacement = 'left' | 'right';

export type AppToolbarProps = {
  placement: AppToolbarPlacement;
  children: React.ReactNode;
};

// The native toolbar renders its items outside the accessibility tree, so nothing
// Maestro can select ever appears — not a testID, which it does not accept, and not
// an accessibility label either. E2E builds swap it for the react-navigation header,
// whose contents are ordinary React Native views and can carry an id.
//
// The same bargain as the authentication provider: the flag cannot reach a production
// build (app.config.ts strips it, CI asserts it), and what the flows then exercise is
// this app's handlers rather than the library's toolbar. budget-period.yaml keeps its
// coordinate tap on purpose, so one flow still drives the real toolbar.
export const AppToolbar = ({ placement, children }: AppToolbarProps) => {
  if (E2eConfig.isEnabled()) {
    const header = () => <View style={{ flexDirection: 'row', alignItems: 'center', gap: 16 }}>{children}</View>;
    return <Stack.Screen options={placement === 'right' ? { headerRight: header } : { headerLeft: header }} />;
  }

  return <Stack.Toolbar placement={placement}>{children}</Stack.Toolbar>;
};
