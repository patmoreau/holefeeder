import { DateTimePicker, DateTimePickerProps } from '@expo/ui/jetpack-compose';

export type ExpoDateTimePickerProps = DateTimePickerProps & {};

export const ExpoDateTimePicker = (props: ExpoDateTimePickerProps) => {
  return <DateTimePicker {...props} />;
};
