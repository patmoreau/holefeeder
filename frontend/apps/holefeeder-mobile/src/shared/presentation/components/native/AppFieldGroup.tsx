import { onScrollPhaseChange, scrollDismissesKeyboard } from '@expo/ui/swift-ui/modifiers';
import { Keyboard, Platform } from 'react-native';
import { ExpoFieldGroup, ExpoFieldGroupProps } from '@/shared/presentation/components/native/expo/ExpoFieldGroup';

export type AppFieldGroupProps = ExpoFieldGroupProps & {};

export const AppFieldGroup = ({ modifiers = [], ...props }: AppFieldGroupProps) => {
  const allModifiers =
    Platform.OS === 'ios'
      ? [
          scrollDismissesKeyboard('immediately'),
          onScrollPhaseChange((phase) => {
            if (phase === 'interacting') Keyboard.dismiss();
          }),
          ...modifiers,
        ]
      : modifiers;
  return <ExpoFieldGroup modifiers={allModifiers} {...props} />;
};
