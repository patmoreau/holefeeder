import { pickerStyle } from '@expo/ui/swift-ui/modifiers';
import { StyleProp, ViewStyle } from 'react-native';
import { ExpoModifierConfig } from '@/shared/presentation/components/app/expo/expo-modifiers';
import { LoadingIndicator } from '@/shared/presentation/components/LoadingIndicator';
import { ExpoPicker } from './expo/ExpoPicker';

export type PickerOption = {
  id: string;
  [key: string]: unknown;
};

export type PickerProps<T extends PickerOption> = {
  variant?: 'menu' | 'segmented';
  options: T[];
  selectedOption: T;
  onSelectOption: (option: T) => void;
  onOptionLabel: (option: T) => string;
  style?: StyleProp<ViewStyle>;
};

export const AppPicker = <T extends PickerOption>({
  variant = 'menu',
  options,
  onOptionLabel,
  selectedOption,
  onSelectOption,
}: PickerProps<T>) => {
  if (!options) {
    return <LoadingIndicator size="small" />;
  }

  const modifiers: ExpoModifierConfig[] = [];
  if (variant === 'segmented') {
    modifiers.push(pickerStyle(variant));
  }
  return (
    <ExpoPicker
      selectedValue={selectedOption.id}
      onValueChange={(id: string) => {
        const selected = options.find((option) => option.id === id);
        if (selected) onSelectOption(selected);
      }}
      modifiers={modifiers}
    >
      {options.map((option) => (
        <ExpoPicker.Item key={option.id} label={onOptionLabel(option)} value={option.id} />
      ))}
    </ExpoPicker>
  );
};
