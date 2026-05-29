import { TextInput, TextInputProps } from '@expo/ui';

export type ExpoTextInputProps = TextInputProps & {};

export const ExpoTextInput = (props: ExpoTextInputProps) => {
  return <TextInput {...props} />;
};
