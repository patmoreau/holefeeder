import { Picker } from '@expo/ui/swift-ui';
import { pickerStyle, tag, padding } from '@expo/ui/swift-ui/modifiers';
import { AppLoadingIndicator } from '@/shared/presentation/components/native/AppLoadingIndicator';
import { AppText } from '@/shared/presentation/components/native/AppText';

export type PickerOption = {
  id: string;
  [key: string]: unknown;
};

export type PickerProps<T extends PickerOption> = {
  options: T[];
  selectedOption: T;
  onSelectOption: (option: T) => void;
  onOptionLabel: (option: T) => string;
  testID?: string;
};

export const AppSegmentedMenu = <T extends PickerOption>({
  options,
  onOptionLabel,
  selectedOption,
  onSelectOption,
  testID,
}: PickerProps<T>) => {
  if (!options) {
    return <AppLoadingIndicator size="small" />;
  }

  return (
    <Picker
      testID={testID}
      modifiers={[pickerStyle('segmented'), padding({ horizontal: 12 })]}
      selection={selectedOption.id}
      onSelectionChange={(id: string) => {
        const selected = options.find((option) => option.id === id);
        if (selected) onSelectOption(selected);
      }}
    >
      {options.map((option) => (
        <AppText key={option.id} modifiers={[tag(option.id)]}>
          {onOptionLabel(option)}
        </AppText>
      ))}
    </Picker>
  );
};
