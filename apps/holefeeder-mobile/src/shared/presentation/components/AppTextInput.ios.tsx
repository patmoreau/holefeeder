import { TextField, useNativeState } from '@expo/ui/swift-ui';
import { autocorrectionDisabled } from '@expo/ui/swift-ui/modifiers';
import { AppHost } from '@/shared/presentation/components/AppHost.ios';
import { AppTextInputProps } from '@/shared/presentation/components/AppTextInput';

export const AppTextInput = ({ placeholder, value, onChangeText }: AppTextInputProps) => {
  const textState = useNativeState(value);

  return (
    <AppHost style={{ flex: 1, width: '100%' }}>
      <TextField text={textState} modifiers={[autocorrectionDisabled(true)]} onTextChange={onChangeText} placeholder={placeholder} />
    </AppHost>
  );
};
