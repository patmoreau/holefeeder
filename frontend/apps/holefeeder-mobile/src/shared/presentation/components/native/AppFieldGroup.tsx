import { scrollDismissesKeyboard } from '@expo/ui/swift-ui/modifiers';
import { Platform } from 'react-native';
import { ExpoFieldGroup, ExpoFieldGroupProps } from '@/shared/presentation/components/native/expo/ExpoFieldGroup';

export type AppFieldGroupProps = ExpoFieldGroupProps & {};

export const AppFieldGroup = ({ modifiers = [], ...props }: AppFieldGroupProps) => {
  const allModifiers = Platform.OS === 'ios' ? [scrollDismissesKeyboard('immediately'), ...modifiers] : modifiers;
  return <ExpoFieldGroup modifiers={allModifiers} {...props} />;
};
