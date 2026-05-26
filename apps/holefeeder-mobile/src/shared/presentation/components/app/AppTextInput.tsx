import { TextInputProps, useNativeState } from '@expo/ui';
import { ExpoTextInput } from '../expo/ExpoTextinput';

export type AppTextInputProps = Omit<TextInputProps, 'value'> & {
  value?: string;
};

export const AppTextInput = ({ value = '', ...props }: AppTextInputProps) => {
  const textState = useNativeState(value);
  return <ExpoTextInput autoCorrect={false} value={textState} {...props} />;
};
