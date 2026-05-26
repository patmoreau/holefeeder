import { TextInput, TextInputProps, useNativeState } from '@expo/ui';
import { AppHost } from '@/shared/presentation/components/AppHost.ios';

export type AppTextInputProps = Omit<TextInputProps, 'value'> & {
  value?: string;
};

export const AppTextInput = ({ value = '', ...props }: AppTextInputProps) => {
  const textState = useNativeState(value);
  return (
    <AppHost style={{ flex: 1, width: '100%' }}>
      <TextInput autoCorrect={false} value={textState} {...props} />
    </AppHost>
  );
};
