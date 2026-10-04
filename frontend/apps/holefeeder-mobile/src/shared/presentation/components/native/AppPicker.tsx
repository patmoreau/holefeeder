import * as IosModifiers from '@expo/ui/swift-ui/modifiers';
import { Platform, StyleProp, ViewStyle } from 'react-native';
import { useTheme } from '@/shared/theme/core/use-theme';
import { ExpoPicker } from './expo/ExpoPicker';
import { ExpoZStack } from './expo/ExpoZStack';

export type PickerOption = {
  id: string;
  [key: string]: unknown;
};

export type PickerProps<T extends PickerOption> = {
  options: T[];
  selectedOption: T;
  onSelectOption: (option: T) => void;
  onOptionLabel: (option: T) => string;
  style?: StyleProp<ViewStyle>;
  testID?: string;
};

export const AppPicker = <T extends PickerOption>({ options, onOptionLabel, selectedOption, onSelectOption, testID }: PickerProps<T>) => {
  const { theme } = useTheme();
  const picker = (
    <ExpoPicker
      testID={testID}
      selectedValue={selectedOption.id}
      onValueChange={(id: string) => {
        const selected = options.find((option) => option.id === id);
        if (selected) onSelectOption(selected);
      }}
    >
      {options.map((option) => (
        <ExpoPicker.Item key={option.id} label={onOptionLabel(option)} value={option.id} />
      ))}
    </ExpoPicker>
  );

  if (Platform.OS !== 'ios') return picker;
  return <ExpoZStack modifiers={[IosModifiers.tint(theme.colors.primary)]}>{picker}</ExpoZStack>;
};
