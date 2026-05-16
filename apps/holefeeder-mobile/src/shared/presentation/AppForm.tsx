import { KeyboardAvoidingView, Platform, ScrollView, type ScrollViewProps } from 'react-native';
import { useStyles } from '@/shared/theme/core/use-styles';
import { Theme } from '@/types/theme/theme';

const createStyles = (theme: Theme) => ({
  form: {
    section: {
      flex: 1,
      flexDirection: 'row' as const,
    },
    backgroundColor: theme.colors.background,
  },
  keyboardAvoidingView: {
    flex: 1,
    backgroundColor: theme.colors.background,
  },
});

export const AppForm = ({ style, ...otherProps }: ScrollViewProps) => {
  const styles = useStyles(createStyles);

  return (
    <KeyboardAvoidingView
      style={styles.keyboardAvoidingView}
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      keyboardVerticalOffset={0}
    >
      <ScrollView
        style={styles.form}
        contentContainerStyle={[style]}
        keyboardShouldPersistTaps="handled"
        {...otherProps}
        removeClippedSubviews={false}
      />
    </KeyboardAvoidingView>
  );
};
