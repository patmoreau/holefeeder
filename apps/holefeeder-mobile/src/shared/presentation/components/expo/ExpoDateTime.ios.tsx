import { DatePicker, DatePickerProps } from '@expo/ui/swift-ui';

export type ExpoDateTimePickerProps = DatePickerProps & {};

export const ExpoDateTimePicker = (props: ExpoDateTimePickerProps) => {
  return <DatePicker {...props} />;
};
