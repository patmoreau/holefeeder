import { Stack } from 'expo-router';
import React from 'react';

type StackToolbarButtonProps = React.ComponentProps<typeof Stack.Toolbar.Button>;

export type AppToolbarButtonProps = Pick<StackToolbarButtonProps, 'icon' | 'onPress' | 'disabled' | 'variant' | 'tintColor'> & {
  /**
   * Required, unlike on the component underneath. These buttons carry an icon and no
   * text, so without a label VoiceOver announces nothing at all — and it is the only
   * handle a Maestro flow has, since expo-router's toolbar accepts no testID and
   * passes no identifier to the native button.
   */
  accessibilityLabel: string;
};

export const AppToolbarButton = ({ accessibilityLabel, ...props }: AppToolbarButtonProps) => (
  <Stack.Toolbar.Button accessibilityLabel={accessibilityLabel} {...props} />
);
