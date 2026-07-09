import { DateTimePicker, DateTimePickerProps } from '@expo/ui/jetpack-compose';
import { ExpoDateTimePickerProps } from '@/shared/presentation/components/native/expo/ExpoDateTime';

export const ExpoDateTimePicker = ({ date, onDateChange, modifiers }: ExpoDateTimePickerProps) => {
  const datePickerProps: DateTimePickerProps = {
    initialDate: date?.toISOString(),
    onDateSelected: onDateChange,
    modifiers,
  };

  return <DateTimePicker {...datePickerProps} />;
};
