import { Stack } from 'expo-router';
import React from 'react';
import { Pressable, Text } from 'react-native';
import { E2eConfig } from '@/shared/auth/core/e2e-config';

type StackToolbarButtonProps = React.ComponentProps<typeof Stack.Toolbar.Button>;

export type AppToolbarButtonProps = Pick<StackToolbarButtonProps, 'icon' | 'onPress' | 'disabled' | 'variant' | 'tintColor'> & {
  /**
   * Required, unlike on the component underneath. These buttons carry an icon and no
   * text, so without a label VoiceOver announces nothing at all.
   */
  accessibilityLabel: string;
  /**
   * Only reaches anything in E2E builds, where this renders as a plain view. The
   * native toolbar accepts no identifier and shows nothing to Maestro.
   */
  testID?: string;
};

// Must be a direct child of AppToolbar: outside it, expo-router does not recognise this
// wrapper as a toolbar item and the header renders empty.
export const AppToolbarButton = ({ accessibilityLabel, testID, ...props }: AppToolbarButtonProps) => {
  if (E2eConfig.isEnabled()) {
    // Label as text rather than the icon: the E2E header is a test fixture, and the
    // words make a failing flow's screenshot readable.
    return (
      <Pressable
        accessibilityLabel={accessibilityLabel}
        accessibilityRole="button"
        disabled={props.disabled}
        onPress={props.onPress}
        testID={testID}
      >
        <Text>{accessibilityLabel}</Text>
      </Pressable>
    );
  }

  return <Stack.Toolbar.Button accessibilityLabel={accessibilityLabel} {...props} />;
};
