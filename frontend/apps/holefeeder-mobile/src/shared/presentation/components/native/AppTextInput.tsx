import { TextInputProps, useNativeState, ObservableState } from '@expo/ui';
import { Platform } from 'react-native';
import { keyboardDoneButton, KeyboardDoneButtonParams } from '@/modules/app-modifiers';
import { ExpoTextInput } from './expo/ExpoTextinput';

export type AppTextInputProps = Omit<TextInputProps, 'value'> & {
  value?: string | ObservableState<string>;
  doneButton?: KeyboardDoneButtonParams;
};

export const AppTextInput = ({ value = '', doneButton, modifiers, ...props }: AppTextInputProps) => {
  const stringValue = typeof value === 'string' ? value : '';
  const nativeState = useNativeState(stringValue);
  const textState = typeof value === 'string' ? nativeState : value;
  const allModifiers = doneButton && Platform.OS === 'ios' ? [...(modifiers ?? []), keyboardDoneButton(doneButton)] : modifiers;

  return <ExpoTextInput autoCorrect={false} value={textState} modifiers={allModifiers} {...props} />;
};
