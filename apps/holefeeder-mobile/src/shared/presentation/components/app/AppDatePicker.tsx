import { DateOnly, today, withDate } from '@holefeeder/shared/core';
import { ExpoDateTimePicker, ExpoDateTimePickerProps } from './expo/ExpoDateTime';

export type AppDatePickerProps = {
  selectedDate: DateOnly | null;
  onDateSelected: (date: DateOnly) => void;
};

export const AppDatePicker = ({ selectedDate, onDateSelected }: AppDatePickerProps) => {
  const datePickerProps: ExpoDateTimePickerProps = {
    onDateSelected: (date: Date) => onDateSelected(withDate(date).toDateOnly()),
    initialDate: selectedDate ?? today(),
  };

  return <ExpoDateTimePicker {...datePickerProps} />;
};
