import { TextInput, TextInputProps } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

export type AppTextInputProps = {
  placeholder: string;
  value: string;
  onChangeText: (value: string) => void;
  onSubmitEditing?: () => void;
  returnKeyType?: TextInputProps['returnKeyType'];
};

const createStyles = (theme: Theme) => ({
  input: {
    ...theme.typography.body,
    color: theme.colors.text,
    placeholderTextColor: theme.colors.secondaryText,
    flex: 1,
    minHeight: 40,
    width: '100%' as const,
  },
});

export const AppTextInput = ({ placeholder, value, onChangeText, onSubmitEditing, returnKeyType }: AppTextInputProps) => {
  const styles = useStyles(createStyles);

  return (
    <TextInput
      value={value}
      onChangeText={onChangeText}
      placeholder={placeholder}
      style={styles.input}
      onSubmitEditing={onSubmitEditing}
      returnKeyType={returnKeyType}
    />
  );
};
