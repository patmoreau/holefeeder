import { TextInputProps, useNativeState, ObservableState } from '@expo/ui';
import { ExpoTextInput } from './expo/ExpoTextinput';

export type AppTextInputProps = Omit<TextInputProps, 'value'> & {
  value?: string | ObservableState<string>;
};

export const AppTextInput = ({ value = '', ...props }: AppTextInputProps) => {
  const stringValue = typeof value === 'string' ? value : '';
  const nativeState = useNativeState(stringValue);
  const textState = typeof value === 'string' ? nativeState : value;

  return <ExpoTextInput autoCorrect={false} value={textState} {...props} />;
};
