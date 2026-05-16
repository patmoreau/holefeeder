import { Picker } from '@react-native-picker/picker';
import React from 'react';
import { Platform, StyleProp, ViewStyle } from 'react-native';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { useStyles } from '@/shared/theme/core/use-styles';
import { borderRadius, spacing } from '@/types/theme/design-tokens';
import { Theme } from '@/types/theme/theme';

export type PickerOption = {
  id: string | number;
  [key: string]: any;
};

export type PickerProps<T extends PickerOption> = {
  variant?: 'menu' | 'segmented';
  options: T[];
  selectedOption: T;
  onSelectOption: (option: T) => void;
  onOptionLabel: (option: T) => string;
  style?: StyleProp<ViewStyle>;
};

const createStyles = (theme: Theme) => ({
  picker: {
    borderWidth: 1,
    borderRadius: borderRadius.md,
    padding: Platform.select({ web: spacing.xs, default: spacing.sm }),
    minHeight: 40,
    backgroundColor: theme.colors.secondaryBackground,
    borderColor: theme.colors.separator,
    color: theme.colors.text,
  },
});

export const AppPicker = <T extends PickerOption>({
  variant = 'menu',
  options,
  selectedOption,
  onSelectOption,
  onOptionLabel,
}: PickerProps<T>) => {
  const styles = useStyles(createStyles);

  if (!options) {
    return <LoadingIndicator size="small" />;
  }

  return (
    <Picker
      mode="dialog"
      style={styles.picker}
      selectedValue={selectedOption?.id}
      onValueChange={(itemValue: string | number) => {
        const selected = options.find((option) => option.id === itemValue);
        if (selected) {
          onSelectOption(selected);
        }
      }}
    >
      {options.map((option) => (
        <Picker.Item key={option.id} label={onOptionLabel(option)} value={option.id} />
      ))}
    </Picker>
  );
};
