import { TextField } from '@expo/ui/swift-ui';
import { autocorrectionDisabled } from '@expo/ui/swift-ui/modifiers';
import * as React from 'react';
import { AppHost } from '@/shared/presentation/components/AppHost.ios';
import { AppTextInputProps } from '@/shared/presentation/components/AppTextInput';

export const AppTextInput = ({ placeholder, value, onChangeText }: AppTextInputProps) => {
  return (
    <AppHost style={{ flex: 1, width: '100%' }}>
      <TextField defaultValue={value} modifiers={[autocorrectionDisabled(true)]} onValueChange={onChangeText} placeholder={placeholder} />
    </AppHost>
  );
};
