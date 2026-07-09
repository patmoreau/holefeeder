import { DateOnly, withDate } from '@holefeeder/shared/core';
import { ExpoDateTimePicker, ExpoDateTimePickerProps } from '@/shared/presentation/components/native/expo/ExpoDateTime';

export type AppDatePickerProps = Omit<ExpoDateTimePickerProps, 'date' | 'onDateChange'> & {
  selectedDate?: DateOnly;
  onDateSelected: (date: DateOnly) => void;
};

export const AppDatePicker = ({ selectedDate, onDateSelected, ...props }: AppDatePickerProps) => {
  const datePickerProps: ExpoDateTimePickerProps = {
    onDateChange: (date: Date) => onDateSelected(withDate(date).toDateOnly()),
    date: selectedDate ? withDate(selectedDate).toDate() : undefined,
    ...props,
  };

  return <ExpoDateTimePicker {...datePickerProps} />;
};
