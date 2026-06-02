import { Picker, PickerItemProps, PickerItemValue, PickerProps } from '@expo/ui';

export type ExpoPickerProps<T extends PickerItemValue = PickerItemValue> = PickerProps<T> & {};

const ExpoPickerBase = <T extends PickerItemValue>(props: ExpoPickerProps<T>) => {
  return <Picker<T> {...props} />;
};

ExpoPickerBase.Item = Picker.Item;

export const ExpoPicker = ExpoPickerBase;

export type { PickerItemProps, PickerItemValue };
